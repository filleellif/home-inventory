using AutoMapper;
using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Queries.Items.GetAllItems;

public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, Result<PaginatedList<ItemDto>>>
{
    private readonly IInventoryItemRepository _itemRepository;
    private readonly IMapper _mapper;

    public GetAllItemsQueryHandler(IInventoryItemRepository itemRepository, IMapper mapper)
    {
        _itemRepository = itemRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ItemDto>>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var items = await _itemRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
            var totalCount = await _itemRepository.GetTotalCountAsync(cancellationToken);

            var itemDtos = _mapper.Map<List<ItemDto>>(items);
            var paginatedList = new PaginatedList<ItemDto>(itemDtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PaginatedList<ItemDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<ItemDto>>.Failure($"Failed to retrieve items: {ex.Message}");
        }
    }
}
