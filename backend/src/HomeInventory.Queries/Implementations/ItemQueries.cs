using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Queries.Interfaces;
using HomeInventory.Queries.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Queries.Implementations;

public class ItemQueries(ApplicationDbContext context) : IItemQueries
{
    public async Task<ItemReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await context.InventoryItems
            .AsNoTracking()
            .Include(x => x.Photos)
            .Include(x => x.Receipts)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (item == null)
            return null;

        return new ItemReadModel
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Quantity = item.Quantity,
            PurchasePriceAmount = item.PurchasePriceAmount,
            PurchasePriceCurrency = item.PurchasePriceCurrency,
            CurrentValueAmount = item.CurrentValueAmount,
            CurrentValueCurrency = item.CurrentValueCurrency,
            PurchaseDate = item.PurchaseDate,
            RoomName = item.RoomName,
            RoomQrCode = item.RoomQrCode,
            ShelfName = item.ShelfName,
            ShelfQrCode = item.ShelfQrCode,
            BoxName = item.BoxName,
            BoxQrCode = item.BoxQrCode,
            CategoryId = item.CategoryId,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            Photos = item.Photos.Select(p => new MediaReferenceReadModel
            {
                Id = p.Id,
                FileName = p.FileName,
                FileUrl = p.FileUrl,
                MediaType = p.MediaType,
                UploadedAt = p.UploadedAt,
                FileSizeBytes = p.FileSizeBytes
            }).ToList(),
            Receipts = item.Receipts.Select(r => new MediaReferenceReadModel
            {
                Id = r.Id,
                FileName = r.FileName,
                FileUrl = r.FileUrl,
                MediaType = r.MediaType,
                UploadedAt = r.UploadedAt,
                FileSizeBytes = r.FileSizeBytes
            }).ToList()
        };
    }

    public async Task<(List<ItemListReadModel> Items, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.InventoryItems.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new ItemListReadModel
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Quantity = i.Quantity,
                CurrentValueAmount = i.CurrentValueAmount,
                CurrentValueCurrency = i.CurrentValueCurrency,
                RoomName = i.RoomName,
                CategoryId = i.CategoryId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<ItemListReadModel>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await context.InventoryItems
            .AsNoTracking()
            .Where(i => i.CategoryId == categoryId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new ItemListReadModel
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Quantity = i.Quantity,
                CurrentValueAmount = i.CurrentValueAmount,
                CurrentValueCurrency = i.CurrentValueCurrency,
                RoomName = i.RoomName,
                CategoryId = i.CategoryId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ItemListReadModel>> GetByQrCodeAsync(string qrCode, CancellationToken cancellationToken = default)
    {
        return await context.InventoryItems
            .AsNoTracking()
            .Where(i => i.RoomQrCode == qrCode ||
                        i.ShelfQrCode == qrCode ||
                        i.BoxQrCode == qrCode)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new ItemListReadModel
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Quantity = i.Quantity,
                CurrentValueAmount = i.CurrentValueAmount,
                CurrentValueCurrency = i.CurrentValueCurrency,
                RoomName = i.RoomName,
                CategoryId = i.CategoryId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
