using HomeInventory.Domain.Aggregates.AreaAggregate;
using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Common;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Commands.Items.UpdateItem;

public class UpdateItemCommandHandler(IInventoryItemRepository itemRepository)
    : ICommandHandler<UpdateItemCommand>
{
    public async Task HandleAsync(UpdateItemCommand command, CancellationToken cancellationToken = default)
    {
        var item = await itemRepository.GetByIdAsync(ItemId.From(command.Id), cancellationToken);

        if (item == null)
        {
            throw new DomainException($"Item with ID {command.Id} not found.");
        }

        // Update basic info
        var basicInfo = ItemBasicInfo.Create(command.Name, command.Description, command.Quantity);
        item.UpdateBasicInfo(basicInfo);

        // Update financial info
        var purchasePrice = command.PurchasePrice.HasValue
            ? Money.Create(command.PurchasePrice.Value, command.PurchaseCurrency ?? "USD")
            : null;

        var currentValue = command.CurrentValue.HasValue
            ? Money.Create(command.CurrentValue.Value, command.CurrentValueCurrency ?? "USD")
            : null;

        var financialInfo = FinancialInfo.Create(purchasePrice, currentValue, command.PurchaseDate);
        item.UpdateFinancialInfo(financialInfo);

        // Update location
        var areaId = command.AreaId.HasValue ? AreaId.From(command.AreaId.Value) : null;
        item.AssignArea(areaId);
        
        // Update category
        var categoryId = command.CategoryId.HasValue ? CategoryId.From(command.CategoryId.Value) : null;
        item.AssignCategory(categoryId);

        await itemRepository.UpdateAsync(item, cancellationToken);
    }
}