using HomeInventory.Application.Commands;
using HomeInventory.Domain.Aggregates.AreaAggregate;
using HomeInventory.Domain.Common;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Commands.Areas.UpdateArea;

public class UpdateAreaCommandHandler(IAreaRepository areaRepository) : ICommandHandler<UpdateAreaCommand>
{
    public async Task HandleAsync(UpdateAreaCommand command, CancellationToken cancellationToken = default)
    {
        var area = await areaRepository.GetByIdAsync(command.Id, cancellationToken);
        if (area == null)
        {
            throw new DomainException($"Area with ID '{command.Id}' not found.");
        }

        // Validate parent exists if specified
        if (command.ParentAreaId.HasValue)
        {
            var parent = await areaRepository.GetByIdAsync(command.ParentAreaId.Value, cancellationToken);
            if (parent == null)
            {
                throw new DomainException($"Parent area with ID '{command.ParentAreaId}' not found.");
            }

            // Prevent circular reference
            if (command.ParentAreaId == command.Id)
            {
                throw new DomainException("An area cannot be its own parent.");
            }
        }

        var parentAreaId = command.ParentAreaId.HasValue ? AreaId.From(command.ParentAreaId.Value) : null;
        
        area.UpdateDetails(command.Name, parentAreaId, command.Description);

        await areaRepository.UpdateAsync(area, cancellationToken);
    }
}
