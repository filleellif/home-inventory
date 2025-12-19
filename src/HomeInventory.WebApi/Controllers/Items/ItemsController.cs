using HomeInventory.Application.Commands;
using HomeInventory.Application.Commands.Items.CreateItem;
using HomeInventory.Application.Commands.Items.DeleteItem;
using HomeInventory.Application.Commands.Items.UpdateItem;
using HomeInventory.Queries.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.WebApi.Controllers.Items;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(
    IItemQueries itemQueries,
    ILogger<ItemsController> logger) : ControllerBase
{
    /// <summary>
    /// Get all inventory items with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        logger.LogInformation("Fetching items - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var (items, totalCount) = await itemQueries.GetAllAsync(pageNumber, pageSize);

        return Ok(new { items, totalCount, pageNumber, pageSize });
    }

    /// <summary>
    /// Get a specific inventory item by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        logger.LogInformation("Fetching item with ID: {ItemId}", id);

        var item = await itemQueries.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound(new { message = "Item not found" });
        }

        return Ok(item);
    }

    /// <summary>
    /// Create a new inventory item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateItemRequest request,
        [FromServices] ICommandHandler<CreateItemCommand> handler)
    {
        logger.LogInformation("Creating new item: {ItemName}", request.Name);

        var itemId = Guid.NewGuid();
        var command = request.ToCommand(itemId);

        await handler.HandleAsync(command);

        return CreatedAtAction(nameof(GetById), new { id = itemId }, new { id = itemId });
    }

    /// <summary>
    /// Update an existing inventory item
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateItemRequest request,
        [FromServices] ICommandHandler<UpdateItemCommand> handler)
    {
        logger.LogInformation("Updating item with ID: {ItemId}", id);

        var command = request.ToCommand(id);

        await handler.HandleAsync(command);

        return Ok(new { message = "Item updated successfully" });
    }

    /// <summary>
    /// Delete an inventory item
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] ICommandHandler<DeleteItemCommand> handler)
    {
        logger.LogInformation("Deleting item with ID: {ItemId}", id);

        var command = new DeleteItemCommand(id);
        await handler.HandleAsync(command);

        return NoContent();
    }
}