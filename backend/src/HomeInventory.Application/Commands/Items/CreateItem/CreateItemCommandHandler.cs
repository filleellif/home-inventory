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
        if (!string.IsNullOrWhiteSpace(command.RoomName) ||
            !string.IsNullOrWhiteSpace(command.ShelfName) ||
            !string.IsNullOrWhiteSpace(command.BoxName) ||
            !string.IsNullOrWhiteSpace(command.RoomQrCode) ||
            !string.IsNullOrWhiteSpace(command.ShelfQrCode) ||
            !string.IsNullOrWhiteSpace(command.BoxQrCode))
        {
            QrCode? roomQr = !string.IsNullOrWhiteSpace(command.RoomQrCode)
                ? QrCode.Create(command.RoomQrCode)
                : null;

            QrCode? shelfQr = !string.IsNullOrWhiteSpace(command.ShelfQrCode)
                ? QrCode.Create(command.ShelfQrCode)
                : null;

            QrCode? boxQr = !string.IsNullOrWhiteSpace(command.BoxQrCode)
                ? QrCode.Create(command.BoxQrCode)
                : null;

            location = Location.Create(
                command.RoomName, roomQr,
                command.ShelfName, shelfQr,
                command.BoxName, boxQr);
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

        // Save to repository
        await itemRepository.AddAsync(item, cancellationToken);
    }
}