using HomeInventory.Queries.ReadModels;

namespace HomeInventory.Queries.Interfaces;

public interface ICategoryQueries
{
    Task<CategoryReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<CategoryReadModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<CategoryReadModel>> GetChildCategoriesAsync(
        Guid parentId,
        CancellationToken cancellationToken = default);
}
