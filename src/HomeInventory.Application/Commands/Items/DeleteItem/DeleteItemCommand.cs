using HomeInventory.Application.Common;
using MediatR;

namespace HomeInventory.Application.Commands.Items.DeleteItem;

public record DeleteItemCommand(Guid Id) : IRequest<Result>;
