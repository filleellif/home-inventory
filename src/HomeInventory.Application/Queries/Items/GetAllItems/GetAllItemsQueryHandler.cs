using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Queries.Items.GetAllItems;

public class GetAllItemsQueryHandler(IInventoryItemRepository itemRepository)
    : IRequestHandler<GetAllItemsQuery, Result<PaginatedList<ItemDto>>>
{
    public async Task<Result<PaginatedList<ItemDto>>> Handle(GetAllItemsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await itemRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
            var totalCount = await itemRepository.GetTotalCountAsync(cancellationToken);

            var itemDtos = items.Select(item => item.FromDomain()).ToList();
            var paginatedList = new PaginatedList<ItemDto>(itemDtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PaginatedList<ItemDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<ItemDto>>.Failure($"Failed to retrieve items: {ex.Message}");
        }
    }
}