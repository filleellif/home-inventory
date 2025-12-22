using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.Aggregates.InventoryItemAggregate.Events;

public record ItemUpdated(ItemId ItemId, DateTime OccurredAt) : IDomainEvent;
