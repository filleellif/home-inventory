using HomeInventory.Domain.Common;

namespace HomeInventory.Domain.Aggregates.InventoryItemAggregate.Events;

public record ItemCreated(ItemId ItemId, DateTime OccurredAt) : IDomainEvent;
