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
    string? RoomName,
    string? RoomQrCode,
    string? ShelfName,
    string? ShelfQrCode,
    string? BoxName,
    string? BoxQrCode,
    Guid? CategoryId
) : Command(Id);