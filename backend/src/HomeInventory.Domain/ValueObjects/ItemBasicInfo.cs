using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.ValueObjects;

public class ItemBasicInfo : ValueObject
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int Quantity { get; private set; }

    // Private parameterless constructor for EF Core
    private ItemBasicInfo()
    {
    }

    private ItemBasicInfo(string name, string? description, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name cannot be empty.");

        if (name.Length > 200)
            throw new DomainException("Item name cannot exceed 200 characters.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Quantity = quantity;
    }

    public static ItemBasicInfo Create(string name, string? description, int quantity)
    {
        return new ItemBasicInfo(name, description, quantity);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Description;
        yield return Quantity;
    }

    public override string ToString()
    {
        return $"{Name} (Qty: {Quantity})";
    }
}
