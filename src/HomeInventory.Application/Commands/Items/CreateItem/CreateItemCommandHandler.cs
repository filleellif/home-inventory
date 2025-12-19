using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Commands.Items.CreateItem;

public class CreateItemCommandHandler(IInventoryItemRepository itemRepository)
    : ICommandHandler<CreateItemCommand>
{
    public async Task HandleAsync(CreateItemCommand command, CancellationToken cancellationToken = default)
    {
        // Create basic info
        var basicInfo = ItemBasicInfo.Create(
            command.Name,
            command.Description,
            command.Quantity
        );

        // Create financial info
        FinancialInfo? financialInfo = null;
        if (command.PurchasePrice.HasValue || command.CurrentValue.HasValue)
        {
            Money? purchasePrice = command.PurchasePrice.HasValue
                ? Money.Create(command.PurchasePrice.Value, command.PurchaseCurrency ?? "USD")
                : null;

            Money? currentValue = command.CurrentValue.HasValue
                ? Money.Create(command.CurrentValue.Value, command.CurrentValueCurrency ?? "USD")
                : null;

            financialInfo = FinancialInfo.Create(purchasePrice, currentValue, command.PurchaseDate);
        }

        // Create location
        Location? location = null;
        if (!string.IsNullOrWhiteSpace(command.Room) ||
            !string.IsNullOrWhiteSpace(command.StorageSpot) ||
            command is { GpsLatitude: not null, GpsLongitude: not null })
        {
            GpsCoordinates? coordinates = null;
            if (command.GpsLatitude.HasValue && command.GpsLongitude.HasValue)
            {
                coordinates = GpsCoordinates.Create(command.GpsLatitude.Value, command.GpsLongitude.Value);
            }

            location = Location.Create(command.Room, command.StorageSpot, coordinates);
        }

        // Create category reference
        var categoryId = command.CategoryId.HasValue
            ? CategoryId.From(command.CategoryId.Value)
            : null;

        // Create the inventory item
        var item = InventoryItem.Create(
            ItemId.From(command.Id),
            basicInfo,
            financialInfo,
            location,
            categoryId
        );

        // Add tags if provided
        if (command.Tags != null && command.Tags.Any())
        {
            foreach (var tagValue in command.Tags.Where(tagValue => !string.IsNullOrWhiteSpace(tagValue)))
            {
                item.AddTag(Tag.Create(tagValue));
            }
        }

        // Save to repository
        await itemRepository.AddAsync(item, cancellationToken);
    }
}