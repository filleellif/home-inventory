using HomeInventory.Domain.Aggregates.AreaAggregate;

namespace HomeInventory.Domain.Repositories;

public interface IAreaRepository
{
    Task<Area?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Area area, CancellationToken cancellationToken = default);
    Task UpdateAsync(Area area, CancellationToken cancellationToken = default);
    Task DeleteAsync(Area area, CancellationToken cancellationToken = default);
}
