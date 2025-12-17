using HomeInventory.Application.DTOs;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;

namespace HomeInventory.Application.Mapping;

public static class ItemMappingExtensions
{
    public static ItemDto FromDomain(this InventoryItem item) => new()
    {
        Id = item.Id.Value,
        Name = item.BasicInfo.Name,
        Description = item.BasicInfo.Description,
        Quantity = item.BasicInfo.Quantity,
        PurchasePrice = item.FinancialInfo.PurchasePrice?.Amount,
        PurchaseCurrency = item.FinancialInfo.PurchasePrice?.Currency,
        CurrentValue = item.FinancialInfo.CurrentValue?.Amount,
        CurrentValueCurrency = item.FinancialInfo.CurrentValue?.Currency,
        PurchaseDate = item.FinancialInfo.PurchaseDate,
        Room = item.Location.Room,
        StorageSpot = item.Location.StorageSpot,
        GpsLatitude = item.Location.Coordinates?.Latitude,
        GpsLongitude = item.Location.Coordinates?.Longitude,
        CategoryId = item.CategoryId?.Value,
        Tags = item.Tags.Select(tag => tag.Value).ToList(),
        Photos = item.Photos.Select(photo => photo.FromDomain()).ToList(),
        Receipts = item.Receipts.Select(photo => photo.FromDomain()).ToList(),
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt,
    };
}