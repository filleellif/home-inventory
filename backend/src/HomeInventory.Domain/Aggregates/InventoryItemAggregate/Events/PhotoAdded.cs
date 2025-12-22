using HomeInventory.Domain.Common;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates.InventoryItemAggregate.Events;

public record PhotoAdded(ItemId ItemId, MediaReference Photo, DateTime OccurredAt) : IDomainEvent;
