using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.Aggregates.CategoryAggregate;

public class Category : AggregateRoot<CategoryId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public CategoryId? ParentCategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core constructor
    private Category() : base(default!)
    {
        Name = default!;
    }

    private Category(CategoryId id, string name, string? description, CategoryId? parentCategoryId)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name cannot be empty.");

        if (name.Length > 100)
            throw new DomainException("Category name cannot exceed 100 characters.");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ParentCategoryId = parentCategoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Category Create(CategoryId id, string name, string? description = null, CategoryId? parentCategoryId = null)
    {
        return new Category(id, name, description, parentCategoryId);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name cannot be empty.");

        if (name.Length > 100)
            throw new DomainException("Category name cannot exceed 100 characters.");

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParentCategory(CategoryId? parentCategoryId)
    {
        if (parentCategoryId != null && parentCategoryId.Equals(Id))
            throw new DomainException("A category cannot be its own parent.");

        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }
}
