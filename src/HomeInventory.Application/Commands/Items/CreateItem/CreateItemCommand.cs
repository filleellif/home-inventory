using HomeInventory.Application.Common;
using MediatR;

namespace HomeInventory.Application.Commands.Items.CreateItem;

public record CreateItemCommand(
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
) : IRequest<Result<Guid>>;