namespace HomeInventory.Application.Commands.Items.CreateItem;

public sealed record CreateItemCommand(
    Guid Id,
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
    List<string>? Tags
) : Command(Id);