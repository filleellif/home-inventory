namespace HomeInventory.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
