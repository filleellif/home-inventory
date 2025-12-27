using HomeInventory.Domain.Aggregates.AreaAggregate;
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

        // Create category reference
        var categoryId = command.CategoryId.HasValue
            ? CategoryId.From(command.CategoryId.Value)
            : null;

        var areaId = command.AreaId.HasValue
            ? AreaId.From(command.AreaId.Value)
            : null;

        // Create the inventory item
        var item = InventoryItem.Create(
            ItemId.From(command.Id),
            basicInfo,
            areaId,
            categoryId
        );

        // Save to repository
        await itemRepository.AddAsync(item, cancellationToken);
    }
}