using HomeInventory.Application.Common;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Commands.Items.DeleteItem;

public class DeleteItemCommandHandler(IInventoryItemRepository itemRepository)
    : IRequestHandler<DeleteItemCommand, Result>
{
    public async Task<Result> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await itemRepository.GetByIdAsync(ItemId.From(request.Id), cancellationToken);

            if (item == null)
            {
                return Result.Failure($"Item with ID {request.Id} not found.");
            }

            await itemRepository.DeleteAsync(ItemId.From(request.Id), cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete item: {ex.Message}");
        }
    }
}
