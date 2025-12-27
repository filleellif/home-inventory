using HomeInventory.Application.Commands.Items.UpdateItem;

namespace HomeInventory.WebApi.Controllers.Items;

public sealed record UpdateItemRequest(
    string Name,
    string? Description,
    int Quantity,
    Guid? AreaId,
    Guid? CategoryId);

internal static class UpdateItemRequestExtensions
{
    internal static UpdateItemCommand ToCommand(this UpdateItemRequest request, Guid id) => new(
        id,
        request.Name,
        request.Description,
        request.Quantity,
        request.AreaId,
        request.CategoryId);
}