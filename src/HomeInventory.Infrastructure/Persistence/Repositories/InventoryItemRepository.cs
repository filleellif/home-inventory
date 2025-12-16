using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetByIdAsync(ItemId id, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<InventoryItem>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryItem>> GetByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .Where(x => x.CategoryId == categoryId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryItem>> GetByTagsAsync(List<Tag> tags, CancellationToken cancellationToken = default)
    {
        var tagValues = tags.Select(t => t.Value).ToList();

        return await _context.InventoryItems
            .Where(item => item.Tags.Any(tag => tagValues.Contains(tag.Value)))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.CountAsync(cancellationToken);
    }

    public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        await _context.InventoryItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ItemId id, CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(id, cancellationToken);
        if (item != null)
        {
            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
