using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Common;
using HomeInventory.Domain.Repositories;

namespace HomeInventory.Application.Commands.Items.DeleteItem;

public class DeleteItemCommandHandler(IInventoryItemRepository itemRepository)
    : ICommandHandler<DeleteItemCommand>
{
    public async Task HandleAsync(DeleteItemCommand command, CancellationToken cancellationToken = default)
    {
        var item = await itemRepository.GetByIdAsync(ItemId.From(command.Id), cancellationToken);

        if (item == null)
        {
            throw new DomainException($"Item with ID {command.Id} not found.");
        }

        await itemRepository.DeleteAsync(ItemId.From(command.Id), cancellationToken);
    }
}