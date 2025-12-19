namespace HomeInventory.Infrastructure.Persistence.Models;

public class InventoryItemModel
{
    public Guid Id { get; init; }

    // BasicInfo flattened
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Quantity { get; init; }

    // FinancialInfo flattened
    public decimal? PurchasePriceAmount { get; init; }
    public string? PurchasePriceCurrency { get; init; }
    public decimal? CurrentValueAmount { get; init; }
    public string? CurrentValueCurrency { get; init; }
    public DateTime? PurchaseDate { get; init; }

    // Location flattened
    public string? Room { get; init; }
    public string? StorageSpot { get; init; }
    public double? GpsLatitude { get; init; }
    public double? GpsLongitude { get; init; }

    public Guid? CategoryId { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Navigation collections (EF will handle separate tables)
    public List<ItemPhotoModel> Photos { get; init; } = [];
    public List<ItemReceiptModel> Receipts { get; init; } = [];
    public List<ItemTagModel> Tags { get; init; } = [];
}
