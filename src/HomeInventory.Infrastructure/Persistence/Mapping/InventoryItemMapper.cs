using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Enums;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

public static class InventoryItemMapper
{
    public static InventoryItem ToDomain(this InventoryItemModel model)
    {
        // Reconstruct BasicInfo value object
        var basicInfo = ItemBasicInfo.Create(
            model.Name,
            model.Description,
            model.Quantity
        );

        // Reconstruct FinancialInfo value object
        FinancialInfo financialInfo;
        if (model.PurchasePriceAmount.HasValue || model.CurrentValueAmount.HasValue || model.PurchaseDate.HasValue)
        {
            Money? purchasePrice = model.PurchasePriceAmount.HasValue
                ? Money.Create(model.PurchasePriceAmount.Value, model.PurchasePriceCurrency ?? "USD")
                : null;

            Money? currentValue = model.CurrentValueAmount.HasValue
                ? Money.Create(model.CurrentValueAmount.Value, model.CurrentValueCurrency ?? "USD")
                : null;

            financialInfo = FinancialInfo.Create(purchasePrice, currentValue, model.PurchaseDate);
        }
        else
        {
            financialInfo = FinancialInfo.Empty();
        }

        // Reconstruct Location value object
        Location location;
        if (!string.IsNullOrWhiteSpace(model.Room) ||
            !string.IsNullOrWhiteSpace(model.StorageSpot) ||
            model.GpsLatitude.HasValue)
        {
            GpsCoordinates? coords = null;
            if (model.GpsLatitude.HasValue && model.GpsLongitude.HasValue)
            {
                coords = GpsCoordinates.Create(model.GpsLatitude.Value, model.GpsLongitude.Value);
            }

            location = Location.Create(model.Room, model.StorageSpot, coords);
        }
        else
        {
            location = Location.Empty();
        }

        // Reconstruct CategoryId
        var categoryId = model.CategoryId.HasValue
            ? CategoryId.From(model.CategoryId.Value)
            : null;

        // Create the domain entity
        var item = InventoryItem.Create(
            ItemId.From(model.Id),
            basicInfo,
            financialInfo,
            location,
            categoryId
        );

        // Add photos via domain methods
        foreach (var photoModel in model.Photos)
        {
            var photo = MediaReference.Create(
                photoModel.FileName,
                photoModel.FileUrl,
                MediaType.Photo,
                photoModel.FileSizeBytes
            );
            item.AddPhoto(photo);
        }

        // Add receipts via domain methods
        foreach (var receiptModel in model.Receipts)
        {
            var receipt = MediaReference.Create(
                receiptModel.FileName,
                receiptModel.FileUrl,
                MediaType.Receipt,
                receiptModel.FileSizeBytes
            );
            item.AddReceipt(receipt);
        }

        // Add tags via domain methods
        foreach (var tagModel in model.Tags)
        {
            var tag = Tag.Create(tagModel.TagValue);
            item.AddTag(tag);
        }

        // Use reflection to set timestamps since they're set in the constructor
        typeof(InventoryItem)
            .GetProperty(nameof(InventoryItem.CreatedAt))!
            .SetValue(item, model.CreatedAt);

        typeof(InventoryItem)
            .GetProperty(nameof(InventoryItem.UpdatedAt))!
            .SetValue(item, model.UpdatedAt);

        return item;
    }

    public static InventoryItemModel ToModel(this InventoryItem domain)
    {
        return new InventoryItemModel
        {
            Id = domain.Id.Value,

            // BasicInfo flattened
            Name = domain.BasicInfo.Name,
            Description = domain.BasicInfo.Description,
            Quantity = domain.BasicInfo.Quantity,

            // FinancialInfo flattened
            PurchasePriceAmount = domain.FinancialInfo.PurchasePrice?.Amount,
            PurchasePriceCurrency = domain.FinancialInfo.PurchasePrice?.Currency,
            CurrentValueAmount = domain.FinancialInfo.CurrentValue?.Amount,
            CurrentValueCurrency = domain.FinancialInfo.CurrentValue?.Currency,
            PurchaseDate = domain.FinancialInfo.PurchaseDate,

            // Location flattened
            Room = domain.Location.Room,
            StorageSpot = domain.Location.StorageSpot,
            GpsLatitude = domain.Location.Coordinates?.Latitude,
            GpsLongitude = domain.Location.Coordinates?.Longitude,

            CategoryId = domain.CategoryId?.Value,

            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,

            // Collections mapped
            Photos = domain.Photos.Select(p => new ItemPhotoModel
            {
                Id = Guid.NewGuid(), // Generate new ID for persistence
                ItemId = domain.Id.Value,
                FileName = p.FileName,
                FileUrl = p.FileUrl,
                MediaType = p.MediaType.ToString(),
                UploadedAt = p.UploadedAt,
                FileSizeBytes = p.FileSizeBytes
            }).ToList(),

            Receipts = domain.Receipts.Select(r => new ItemReceiptModel
            {
                Id = Guid.NewGuid(), // Generate new ID for persistence
                ItemId = domain.Id.Value,
                FileName = r.FileName,
                FileUrl = r.FileUrl,
                MediaType = r.MediaType.ToString(),
                UploadedAt = r.UploadedAt,
                FileSizeBytes = r.FileSizeBytes
            }).ToList(),

            Tags = domain.Tags.Select(t => new ItemTagModel
            {
                ItemId = domain.Id.Value,
                TagValue = t.Value
            }).ToList()
        };
    }
}
