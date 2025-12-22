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
    string? Room,
    string? StorageSpot,
    double? GpsLatitude,
    double? GpsLongitude,
    Guid? CategoryId,
    List<string>? Tags);

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
        request.Room,
        request.StorageSpot,
        request.GpsLatitude,
        request.GpsLongitude,
        request.CategoryId,
        request.Tags);
}