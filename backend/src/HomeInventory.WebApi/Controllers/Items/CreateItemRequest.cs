using HomeInventory.Application.Commands.Items.CreateItem;

namespace HomeInventory.WebApi.Controllers.Items;

public sealed record CreateItemRequest(
    string Name,
    string? Description,
    int Quantity,
    decimal? PurchasePrice,
    string? PurchaseCurrency,
    decimal? CurrentValue,
    string? CurrentValueCurrency,
    DateTime? PurchaseDate,
    Guid? AreaId,
    Guid? CategoryId);

internal static class CreateItemRequestExtensions
{
    internal static CreateItemCommand ToCommand(this CreateItemRequest request, Guid id) => new(
        id,
        request.Name,
        request.Description,
        request.Quantity,
        request.PurchasePrice,
        request.PurchaseCurrency,
        request.CurrentValue,
        request.CurrentValueCurrency,
        request.PurchaseDate,
        request.AreaId,
        request.CategoryId);
}