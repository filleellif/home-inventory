using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Queries.Items.GetItemById;

public class GetItemByIdQueryHandler(IInventoryItemRepository itemRepository)
    : IRequestHandler<GetItemByIdQuery, Result<ItemDto>>
{
    public async Task<Result<ItemDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await itemRepository.GetByIdAsync(ItemId.From(request.Id), cancellationToken);

            if (item == null)
            {
                return Result<ItemDto>.Failure($"Item with ID {request.Id} not found.");
            }

            var itemDto = item.FromDomain();
            return Result<ItemDto>.Success(itemDto);
        }
        catch (Exception ex)
        {
            return Result<ItemDto>.Failure($"Failed to retrieve item: {ex.Message}");
        }
    }
}
