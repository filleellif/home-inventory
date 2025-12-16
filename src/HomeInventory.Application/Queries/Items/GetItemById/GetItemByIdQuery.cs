using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using MediatR;

namespace HomeInventory.Application.Queries.Items.GetItemById;

public record GetItemByIdQuery(Guid Id) : IRequest<Result<ItemDto>>;
