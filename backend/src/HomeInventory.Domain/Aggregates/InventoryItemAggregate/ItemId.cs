namespace HomeInventory.Domain.Aggregates.InventoryItemAggregate;

public record ItemId(Guid Value)
{
    public static ItemId New() => new(Guid.NewGuid());
    public static ItemId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
