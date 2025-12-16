using AutoMapper;
using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Queries.Items.GetItemById;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result<ItemDto>>
{
    private readonly IInventoryItemRepository _itemRepository;
    private readonly IMapper _mapper;

    public GetItemByIdQueryHandler(IInventoryItemRepository itemRepository, IMapper mapper)
    {
        _itemRepository = itemRepository;
        _mapper = mapper;
    }

    public async Task<Result<ItemDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _itemRepository.GetByIdAsync(ItemId.From(request.Id), cancellationToken);

            if (item == null)
            {
                return Result<ItemDto>.Failure($"Item with ID {request.Id} not found.");
            }

            var itemDto = _mapper.Map<ItemDto>(item);
            return Result<ItemDto>.Success(itemDto);
        }
        catch (Exception ex)
        {
            return Result<ItemDto>.Failure($"Failed to retrieve item: {ex.Message}");
        }
    }
}
