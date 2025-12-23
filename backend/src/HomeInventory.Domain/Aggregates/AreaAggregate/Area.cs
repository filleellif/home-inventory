using HomeInventory.Domain.Common;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates.AreaAggregate;

public class Area : AggregateRoot<AreaId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public AreaId? ParentAreaId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Area(AreaId id, string name, AreaId? parentAreaId = null, string? description = null) : base(id)
    {
        Name = name;
        ParentAreaId = parentAreaId;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Area Create(AreaId id, string name, AreaId? parentAreaId = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Area name cannot be empty.");

        if (name.Length > 200)
            throw new DomainException("Area name cannot exceed 200 characters.");

        return new Area(id, name, parentAreaId, description);
    }

    public void UpdateDetails(string name, AreaId? parentAreaId, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Area name cannot be empty.");

        if (name.Length > 200)
            throw new DomainException("Area name cannot exceed 200 characters.");

        Name = name;
        ParentAreaId = parentAreaId;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
