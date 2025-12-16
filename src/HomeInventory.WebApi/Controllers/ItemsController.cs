using HomeInventory.Application.Commands.Items.CreateItem;
using HomeInventory.Application.Commands.Items.DeleteItem;
using HomeInventory.Application.Commands.Items.UpdateItem;
using HomeInventory.Application.Queries.Items.GetAllItems;
using HomeInventory.Application.Queries.Items.GetItemById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(IMediator mediator, ILogger<ItemsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all inventory items with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Fetching items - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var query = new GetAllItemsQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Get a specific inventory item by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Fetching item with ID: {ItemId}", id);

        var query = new GetItemByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess || result.Value == null)
        {
            return NotFound(new { message = result.Error ?? "Item not found" });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new inventory item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateItemCommand command)
    {
        _logger.LogInformation("Creating new item: {ItemName}", command.Name);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });
    }

    /// <summary>
    /// Update an existing inventory item
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID in URL does not match ID in request body" });
        }

        _logger.LogInformation("Updating item with ID: {ItemId}", id);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new { message = result.Error });
            }
            return BadRequest(new { message = result.Error });
        }

        return Ok(new { message = "Item updated successfully" });
    }

    /// <summary>
    /// Delete an inventory item
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting item with ID: {ItemId}", id);

        var command = new DeleteItemCommand(id);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Error });
        }

        return NoContent();
    }
}
