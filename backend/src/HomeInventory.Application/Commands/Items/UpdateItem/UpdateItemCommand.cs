namespace HomeInventory.Application.Commands.Items.UpdateItem;

public record UpdateItemCommand(
    Guid Id,
    string Name,
    string? Description,
    int Quantity,
    decimal? PurchasePrice,
    string? PurchaseCurrency,
    decimal? CurrentValue,
    string? CurrentValueCurrency,
    DateTime? PurchaseDate,
    Guid? AreaId,
    Guid? CategoryId
) : Command(Id);
