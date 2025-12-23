using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Queries.Interfaces;
using HomeInventory.Queries.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Queries.Implementations;

public class AreaQueries : IAreaQueries
{
    private readonly ApplicationDbContext _context;

    public AreaQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AreaReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var areas = await _context.Areas
            .AsNoTracking()
            .Include(x => x.ParentArea)
            .Include(x => x.ChildAreas)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = new List<AreaReadModel>();

        foreach (var area in areas)
        {
            var fullPath = await BuildFullPathAsync(area.Id, cancellationToken);

            var itemCount = await _context.InventoryItems
                .Where(i => i.AreaId == area.Id)
                .CountAsync(cancellationToken);

            result.Add(new AreaReadModel
            {
                Id = area.Id,
                Name = area.Name,
                Description = area.Description,
                ParentAreaId = area.ParentAreaId,
                ParentAreaName = area.ParentArea?.Name,
                FullPath = fullPath,
                ItemCount = itemCount,
                ChildCount = area.ChildAreas.Count,
                CreatedAt = area.CreatedAt,
                UpdatedAt = area.UpdatedAt
            });
        }

        return result;
    }

    private async Task<string> BuildFullPathAsync(Guid areaId, CancellationToken cancellationToken)
    {
        var path = new List<string>();
        var currentId = areaId;

        while (currentId != Guid.Empty)
        {
            var area = await _context.Areas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == currentId, cancellationToken);

            if (area == null)
                break;

            path.Insert(0, area.Name);
            currentId = area.ParentAreaId ?? Guid.Empty;
        }

        return string.Join(" > ", path);
    }

    public async Task<AreaReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var area = await _context.Areas
            .AsNoTracking()
            .Include(x => x.ParentArea)
            .Include(x => x.ChildAreas)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (area == null)
            return null;

        var fullPath = await BuildFullPathAsync(area.Id, cancellationToken);

        var itemCount = await _context.InventoryItems
            .Where(i => i.AreaId == area.Id)
            .CountAsync(cancellationToken);

        return new AreaReadModel
        {
            Id = area.Id,
            Name = area.Name,
            Description = area.Description,
            ParentAreaId = area.ParentAreaId,
            ParentAreaName = area.ParentArea?.Name,
            FullPath = fullPath,
            ItemCount = itemCount,
            ChildCount = area.ChildAreas.Count,
            CreatedAt = area.CreatedAt,
            UpdatedAt = area.UpdatedAt
        };
    }
}