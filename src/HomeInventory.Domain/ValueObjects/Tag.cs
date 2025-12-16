using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.ValueObjects;

public class Tag : ValueObject
{
    public string Value { get; }

    private Tag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Tag value cannot be empty.");

        if (value.Length > 50)
            throw new DomainException("Tag value cannot exceed 50 characters.");

        Value = value.Trim().ToLowerInvariant();
    }

    public static Tag Create(string value)
    {
        return new Tag(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
