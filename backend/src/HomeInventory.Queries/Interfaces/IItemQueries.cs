using HomeInventory.Queries.ReadModels;

namespace HomeInventory.Queries.Interfaces;

public interface IItemQueries
{
    Task<ItemReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(List<ItemListReadModel> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<List<ItemListReadModel>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);

    Task<List<ItemListReadModel>> GetByQrCodeAsync(
        string qrCode,
        CancellationToken cancellationToken = default);
}
