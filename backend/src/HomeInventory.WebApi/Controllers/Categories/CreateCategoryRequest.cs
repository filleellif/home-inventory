using HomeInventory.Application.Commands.Categories.CreateCategory;

namespace HomeInventory.WebApi.Controllers.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description,
    Guid? ParentCategoryId);

internal static class CreateCategoryRequestExtensions
{
    internal static CreateCategoryCommand ToCommand(this CreateCategoryRequest request, Guid id) => new(
        id,
        request.Name,
        request.Description,
        request.ParentCategoryId);
}