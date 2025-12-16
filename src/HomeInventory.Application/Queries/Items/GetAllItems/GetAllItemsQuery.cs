using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using MediatR;

namespace HomeInventory.Application.Queries.Items.GetAllItems;

public record GetAllItemsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedList<ItemDto>>>;
