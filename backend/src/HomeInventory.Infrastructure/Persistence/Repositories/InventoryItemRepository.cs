using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Repositories;

public class InventoryItemRepository(ApplicationDbContext context) : IInventoryItemRepository
{
    public async Task<InventoryItem?> GetByIdAsync(ItemId id, CancellationToken cancellationToken = default)
    {
        var model = await context.InventoryItems
            .Include(x => x.Photos)
            .Include(x => x.Receipts)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

        return model?.ToDomain();
    }

    public async Task<List<InventoryItem>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var models = await context.InventoryItems
            .Include(x => x.Photos)
            .Include(x => x.Receipts)
            .Include(x => x.Tags)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<List<InventoryItem>> GetByCategoryAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        var models = await context.InventoryItems
            .Include(x => x.Photos)
            .Include(x => x.Receipts)
            .Include(x => x.Tags)
            .Where(x => x.CategoryId == categoryId.Value)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<List<InventoryItem>> GetByTagsAsync(List<Tag> tags, CancellationToken cancellationToken = default)
    {
        var tagValues = tags.Select(t => t.Value).ToList();

        var models = await context.InventoryItems
            .Include(x => x.Photos)
            .Include(x => x.Receipts)
            .Include(x => x.Tags)
            .Where(item => item.Tags.Any(tag => tagValues.Contains(tag.TagValue)))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await context.InventoryItems.CountAsync(cancellationToken);
    }

    public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        var model = item.ToModel();
        await context.InventoryItems.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        var model = item.ToModel();

        // First, remove existing related entities
        var existingPhotos = await context.ItemPhotos
            .Where(p => p.ItemId == item.Id.Value)
            .ToListAsync(cancellationToken);
        context.ItemPhotos.RemoveRange(existingPhotos);

        var existingReceipts = await context.ItemReceipts
            .Where(r => r.ItemId == item.Id.Value)
            .ToListAsync(cancellationToken);
        context.ItemReceipts.RemoveRange(existingReceipts);

        var existingTags = await context.ItemTags
            .Where(t => t.ItemId == item.Id.Value)
            .ToListAsync(cancellationToken);
        context.ItemTags.RemoveRange(existingTags);

        // Update the main item
        context.InventoryItems.Update(model);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ItemId id, CancellationToken cancellationToken = default)
    {
        var model = await context.InventoryItems
            .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

        if (model != null)
        {
            context.InventoryItems.Remove(model);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
