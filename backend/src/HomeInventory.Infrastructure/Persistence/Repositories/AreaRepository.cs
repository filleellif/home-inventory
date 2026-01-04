using HomeInventory.Domain.Aggregates.AreaAggregate;
using HomeInventory.Domain.Repositories;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Repositories;

public class AreaRepository(ApplicationDbContext context) : IAreaRepository
{
    public async Task<Area?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await context.Areas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model != null ? ToDomain(model) : null;
    }

    public async Task AddAsync(Area area, CancellationToken cancellationToken = default)
    {
        var model = ToModel(area);
        await context.Areas.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Area area, CancellationToken cancellationToken = default)
    {
        var model = ToModel(area);
        context.Areas.Update(model);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Area area, CancellationToken cancellationToken = default)
    {
        var model = await context.Areas.FirstOrDefaultAsync(a => a.Id == area.Id.Value, cancellationToken);
        if (model != null)
        {
            context.Areas.Remove(model);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static Area ToDomain(AreaModel model)
    {
        var areaId = AreaId.From(model.Id);
        var parentAreaId = model.ParentAreaId.HasValue
            ? AreaId.From(model.ParentAreaId.Value)
            : null;

        var area = Area.Create(areaId, model.Name, parentAreaId, model.Description);

        // Use reflection to set the base class timestamps since they don't have setters
        var createdAtProperty = typeof(Area).GetProperty("CreatedAt")!;
        var updatedAtProperty = typeof(Area).GetProperty("UpdatedAt")!;
        createdAtProperty.SetValue(area, model.CreatedAt);
        updatedAtProperty.SetValue(area, model.UpdatedAt);

        return area;
    }

    private static AreaModel ToModel(Area domain)
    {
        return new AreaModel
        {
            Id = domain.Id.Value,
            Name = domain.Name,
            Description = domain.Description,
            ParentAreaId = domain.ParentAreaId?.Value,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}