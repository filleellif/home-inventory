using HomeInventory.Application.Commands;
using HomeInventory.Domain.Aggregates.AreaAggregate;
using HomeInventory.Domain.Common;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Commands.Areas.CreateArea;

public class CreateAreaCommandHandler(IAreaRepository areaRepository) : ICommandHandler<CreateAreaCommand>
{
    public async Task HandleAsync(CreateAreaCommand command, CancellationToken cancellationToken = default)
    {
        // Validate parent exists if specified
        if (command.ParentAreaId.HasValue)
        {
            var parent = await areaRepository.GetByIdAsync(command.ParentAreaId.Value, cancellationToken);
            if (parent == null)
            {
                throw new DomainException($"Parent area with ID '{command.ParentAreaId}' not found.");
            }
        }

        var parentAreaId = command.ParentAreaId.HasValue
            ? AreaId.From(command.ParentAreaId.Value)
            : null;

        var area = Area.Create(AreaId.From(command.Id), command.Name, parentAreaId, command.Description);

        await areaRepository.AddAsync(area, cancellationToken);
    }
}