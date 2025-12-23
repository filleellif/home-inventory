namespace HomeInventory.Domain.Aggregates.AreaAggregate;

public record AreaId(Guid Value)
{
    public static AreaId New() => new(Guid.NewGuid());
    public static AreaId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
