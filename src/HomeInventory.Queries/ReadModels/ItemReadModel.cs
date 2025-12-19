namespace HomeInventory.Queries.ReadModels;

public class ItemReadModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Quantity { get; init; }

    public decimal? PurchasePriceAmount { get; init; }
    public string? PurchasePriceCurrency { get; init; }
    public decimal? CurrentValueAmount { get; init; }
    public string? CurrentValueCurrency { get; init; }
    public DateTime? PurchaseDate { get; init; }

    public string? Room { get; init; }
    public string? StorageSpot { get; init; }
    public double? GpsLatitude { get; init; }
    public double? GpsLongitude { get; init; }

    public Guid? CategoryId { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public List<MediaReferenceReadModel> Photos { get; init; } = new();
    public List<MediaReferenceReadModel> Receipts { get; init; } = new();
    public List<string> Tags { get; init; } = new();
}
