using HomeInventory.Queries.ReadModels;

namespace HomeInventory.Queries.Interfaces;

public interface IAreaQueries
{
    Task<List<AreaReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AreaReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
