using HomeInventory.Application.Commands.Categories.CreateCategory;
using HomeInventory.Application.Queries.Categories.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(IMediator mediator, ILogger<CategoriesController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Fetching all categories");

        var query = new GetAllCategoriesQuery();
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        logger.LogInformation("Creating new category: {CategoryName}", command.Name);

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value });
    }
}