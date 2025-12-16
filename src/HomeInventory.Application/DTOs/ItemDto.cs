namespace HomeInventory.Application.DTOs;

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? PurchaseCurrency { get; set; }
    public decimal? CurrentValue { get; set; }
    public string? CurrentValueCurrency { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public string? Room { get; set; }
    public string? StorageSpot { get; set; }
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }
    public Guid? CategoryId { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<MediaReferenceDto> Photos { get; set; } = new();
    public List<MediaReferenceDto> Receipts { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
