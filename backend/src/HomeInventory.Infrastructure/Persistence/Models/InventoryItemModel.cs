namespace HomeInventory.Infrastructure.Persistence.Models;

public class InventoryItemModel
{
    public Guid Id { get; init; }

    // BasicInfo flattened
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Quantity { get; init; }

    // Location flattened
    public Guid? AreaId { get; init; }

    public Guid? CategoryId { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Navigation collections (EF will handle separate tables)
    public List<ItemPhotoModel> Photos { get; init; } = [];
    public List<ItemReceiptModel> Receipts { get; init; } = [];
}
