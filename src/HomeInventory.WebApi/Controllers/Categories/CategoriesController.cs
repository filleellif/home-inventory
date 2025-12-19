using HomeInventory.Application.Commands;
using HomeInventory.Application.Commands.Categories.CreateCategory;
using HomeInventory.Queries.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.WebApi.Controllers.Categories;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(
    ICategoryQueries categoryQueries,
    ILogger<CategoriesController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Fetching all categories");

        var categories = await categoryQueries.GetAllAsync();

        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ICommandHandler<CreateCategoryCommand> handler)
    {
        logger.LogInformation("Creating new category: {CategoryName}", request.Name);

        var categoryId = Guid.NewGuid();
        var command = request.ToCommand(categoryId);
        await handler.HandleAsync(command);

        return CreatedAtAction(nameof(GetAll), new { id = categoryId }, new { id = categoryId });
    }
}