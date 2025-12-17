using HomeInventory.Application.Common;
using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;
using MediatR;

namespace HomeInventory.Application.Commands.Items.CreateItem;

public class CreateItemCommandHandler(IInventoryItemRepository itemRepository)
    : IRequestHandler<CreateItemCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Create basic info
            var basicInfo = ItemBasicInfo.Create(
                request.Name,
                request.Description,
                request.Quantity
            );

            // Create financial info
            FinancialInfo? financialInfo = null;
            if (request.PurchasePrice.HasValue || request.CurrentValue.HasValue)
            {
                Money? purchasePrice = request.PurchasePrice.HasValue
                    ? Money.Create(request.PurchasePrice.Value, request.PurchaseCurrency ?? "USD")
                    : null;

                Money? currentValue = request.CurrentValue.HasValue
                    ? Money.Create(request.CurrentValue.Value, request.CurrentValueCurrency ?? "USD")
                    : null;

                financialInfo = FinancialInfo.Create(purchasePrice, currentValue, request.PurchaseDate);
            }

            // Create location
            Location? location = null;
            if (!string.IsNullOrWhiteSpace(request.Room) ||
                !string.IsNullOrWhiteSpace(request.StorageSpot) ||
                request is { GpsLatitude: not null, GpsLongitude: not null })
            {
                GpsCoordinates? coordinates = null;
                if (request.GpsLatitude.HasValue && request.GpsLongitude.HasValue)
                {
                    coordinates = GpsCoordinates.Create(request.GpsLatitude.Value, request.GpsLongitude.Value);
                }

                location = Location.Create(request.Room, request.StorageSpot, coordinates);
            }

            // Create category reference
            var categoryId = request.CategoryId.HasValue
                ? CategoryId.From(request.CategoryId.Value)
                : null;

            // Create the inventory item
            var item = InventoryItem.Create(
                ItemId.New(),
                basicInfo,
                financialInfo,
                location,
                categoryId
            );

            // Add tags if provided
            if (request.Tags != null && request.Tags.Any())
            {
                foreach (var tagValue in request.Tags.Where(tagValue => !string.IsNullOrWhiteSpace(tagValue)))
                {
                    item.AddTag(Tag.Create(tagValue));
                }
            }

            // Save to repository
            await itemRepository.AddAsync(item, cancellationToken);

            return Result<Guid>.Success(item.Id.Value);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create item: {ex.Message}");
        }
    }
}