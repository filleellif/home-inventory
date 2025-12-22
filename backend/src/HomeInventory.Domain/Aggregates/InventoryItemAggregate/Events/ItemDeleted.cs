using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.Aggregates.InventoryItemAggregate.Events;

public record ItemDeleted(ItemId ItemId, DateTime OccurredAt) : IDomainEvent;
