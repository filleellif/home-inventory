using HomeInventory.Application.Common;
using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;
using MediatR;

namespace HomeInventory.Application.Commands.Items.UpdateItem;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Result>
{
    private readonly IInventoryItemRepository _itemRepository;

    public UpdateItemCommandHandler(IInventoryItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(ItemId.From(request.Id), cancellationToken);

            if (item == null)
            {
                return Result.Failure($"Item with ID {request.Id} not found.");
            }

            // Update basic info
            var basicInfo = ItemBasicInfo.Create(request.Name, request.Description, request.Quantity);
            item.UpdateBasicInfo(basicInfo);

            // Update financial info
            Money? purchasePrice = request.PurchasePrice.HasValue
                ? Money.Create(request.PurchasePrice.Value, request.PurchaseCurrency ?? "USD")
                : null;

            Money? currentValue = request.CurrentValue.HasValue
                ? Money.Create(request.CurrentValue.Value, request.CurrentValueCurrency ?? "USD")
                : null;

            var financialInfo = FinancialInfo.Create(purchasePrice, currentValue, request.PurchaseDate);
            item.UpdateFinancialInfo(financialInfo);

            // Update location
            GpsCoordinates? coordinates = null;
            if (request.GpsLatitude.HasValue && request.GpsLongitude.HasValue)
            {
                coordinates = GpsCoordinates.Create(request.GpsLatitude.Value, request.GpsLongitude.Value);
            }

            var location = Location.Create(request.Room, request.StorageSpot, coordinates);
            item.UpdateLocation(location);

            // Update category
            var categoryId = request.CategoryId.HasValue ? CategoryId.From(request.CategoryId.Value) : null;
            item.AssignCategory(categoryId);

            // Update tags
            item.ClearTags();
            if (request.Tags != null && request.Tags.Any())
            {
                foreach (var tagValue in request.Tags)
                {
                    if (!string.IsNullOrWhiteSpace(tagValue))
                    {
                        item.AddTag(Tag.Create(tagValue));
                    }
                }
            }

            await _itemRepository.UpdateAsync(item, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update item: {ex.Message}");
        }
    }
}
