using HomeInventory.Application.Commands.Items.CreateItem;

namespace HomeInventory.WebApi.Controllers.Items;

public sealed record CreateItemRequest(
    string Name,
    string? Description,
    int Quantity,
    Guid? AreaId,
    Guid? CategoryId);

internal static class CreateItemRequestExtensions
{
    internal static CreateItemCommand ToCommand(this CreateItemRequest request, Guid id) => new(
        id,
        request.Name,
        request.Description,
        request.Quantity,
        request.AreaId,
        request.CategoryId);
}