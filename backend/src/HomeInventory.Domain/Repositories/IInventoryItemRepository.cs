using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Repositories;

public interface IInventoryItemRepository
{
    Task<InventoryItem?> GetByIdAsync(ItemId id, CancellationToken cancellationToken = default);
    Task<List<InventoryItem>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<List<InventoryItem>> GetByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(ItemId id, CancellationToken cancellationToken = default);
}
