using HomeInventory.Application.Commands;
using HomeInventory.Domain.Common;
using HomeInventory.Domain.Repositories;

namespace HomeInventory.Application.Commands.Areas.DeleteArea;

public class DeleteAreaCommandHandler(IAreaRepository areaRepository) : ICommandHandler<DeleteAreaCommand>
{
    public async Task HandleAsync(DeleteAreaCommand command, CancellationToken cancellationToken = default)
    {
        var area = await areaRepository.GetByIdAsync(command.Id, cancellationToken);
        if (area == null)
        {
            throw new DomainException($"Area with ID '{command.Id}' not found.");
        }

        await areaRepository.DeleteAsync(area, cancellationToken);
    }
}