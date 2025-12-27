using HomeInventory.Domain.Aggregates.AreaAggregate;
using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate.Events;
using HomeInventory.Domain.Common;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates.InventoryItemAggregate;

public class InventoryItem : AggregateRoot<ItemId>
{
    private readonly List<MediaReference> _photos = new();
    private readonly List<MediaReference> _receipts = new();

    public ItemBasicInfo BasicInfo { get; private set; }
    public AreaId? AreaId { get; private set; }
    public CategoryId? CategoryId { get; private set; }

    public IReadOnlyCollection<MediaReference> Photos => _photos.AsReadOnly();
    public IReadOnlyCollection<MediaReference> Receipts => _receipts.AsReadOnly();

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core constructor
    private InventoryItem() : base(default!)
    {
        BasicInfo = default!;
    }

    private InventoryItem(
        ItemId id,
        ItemBasicInfo basicInfo,
        AreaId? areaId,
        CategoryId? categoryId)
        : base(id)
    {
        BasicInfo = basicInfo ?? throw new ArgumentNullException(nameof(basicInfo));
        AreaId = areaId;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ItemCreated(id, DateTime.UtcNow));
    }

    public static InventoryItem Create(
        ItemId id,
        ItemBasicInfo basicInfo,
        AreaId? areaId = null,
        CategoryId? categoryId = null)
    {
        return new InventoryItem(id, basicInfo, areaId, categoryId);
    }

    public void UpdateBasicInfo(ItemBasicInfo basicInfo)
    {
        BasicInfo = basicInfo ?? throw new ArgumentNullException(nameof(basicInfo));
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemUpdated(Id, DateTime.UtcNow));
    }

    public void AssignArea(AreaId? areaId)
    {
        AreaId = areaId;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemUpdated(Id, DateTime.UtcNow));
    }

    public void AssignCategory(CategoryId? categoryId)
    {
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemUpdated(Id, DateTime.UtcNow));
    }

    public void AddPhoto(MediaReference photo)
    {
        if (photo == null)
            throw new ArgumentNullException(nameof(photo));

        if (photo.MediaType != Enums.MediaType.Photo)
            throw new DomainException("Only photos can be added using AddPhoto method.");

        _photos.Add(photo);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new PhotoAdded(Id, photo, DateTime.UtcNow));
    }

    public void RemovePhoto(MediaReference photo)
    {
        if (photo == null)
            throw new ArgumentNullException(nameof(photo));

        _photos.Remove(photo);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemUpdated(Id, DateTime.UtcNow));
    }

    public void AddReceipt(MediaReference receipt)
    {
        if (receipt == null)
            throw new ArgumentNullException(nameof(receipt));

        if (receipt.MediaType != Enums.MediaType.Receipt)
            throw new DomainException("Only receipts can be added using AddReceipt method.");

        _receipts.Add(receipt);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemUpdated(Id, DateTime.UtcNow));
    }

    public void RemoveReceipt(MediaReference receipt)
    {
        if (receipt == null)
            throw new ArgumentNullException(nameof(receipt));

        _receipts.Remove(receipt);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ItemUpdated(Id, DateTime.UtcNow));
    }
}
