using HomeInventory.Application.Commands.Items.UpdateItem;

namespace HomeInventory.WebApi.Controllers.Items;

public sealed record UpdateItemRequest(
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

internal static class UpdateItemRequestExtensions
{
    internal static UpdateItemCommand ToCommand(this UpdateItemRequest request, Guid id) => new(
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