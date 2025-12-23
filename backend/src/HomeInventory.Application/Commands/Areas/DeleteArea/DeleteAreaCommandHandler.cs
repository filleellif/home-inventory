using HomeInventory.Application.Commands;
using HomeInventory.Domain.Common;
using HomeInventory.Domain.Repositories;

namespace HomeInventory.Application.Commands.Areas.DeleteArea;

public class DeleteAreaCommandHandler : ICommandHandler<DeleteAreaCommand>
{
    private readonly IAreaRepository _areaRepository;

    public DeleteAreaCommandHandler(IAreaRepository areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public async Task HandleAsync(DeleteAreaCommand command, CancellationToken cancellationToken = default)
    {
        var area = await _areaRepository.GetByIdAsync(command.Id, cancellationToken);
        if (area == null)
        {
            throw new DomainException($"Area with ID '{command.Id}' not found.");
        }

        await _areaRepository.DeleteAsync(area, cancellationToken);
    }
}
