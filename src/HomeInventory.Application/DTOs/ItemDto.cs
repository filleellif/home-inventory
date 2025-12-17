namespace HomeInventory.Application.DTOs;

public class ItemDto
{
    public Guid Id { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public string? Description { get; init; }
    
    public int Quantity { get; init; }
    
    public decimal? PurchasePrice { get; init; }
    
    public string? PurchaseCurrency { get; init; }
    
    public decimal? CurrentValue { get; init; }
    
    public string? CurrentValueCurrency { get; init; }
    
    public DateTime? PurchaseDate { get; init; }
    
    public string? Room { get; init; }
    
    public string? StorageSpot { get; init; }
    
    public double? GpsLatitude { get; init; }
    
    public double? GpsLongitude { get; init; }
    
    public Guid? CategoryId { get; init; }
    
    public List<string> Tags { get; init; } = new();
    
    public List<MediaReferenceDto> Photos { get; init; } = new();
    
    public List<MediaReferenceDto> Receipts { get; init; } = new();
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime UpdatedAt { get; init; }
}
