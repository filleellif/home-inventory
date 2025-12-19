using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Queries.Interfaces;
using HomeInventory.Queries.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Queries.Implementations;

public class CategoryQueries(ApplicationDbContext context) : ICategoryQueries
{
    public async Task<CategoryReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category == null)
            return null;

        return new CategoryReadModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<List<CategoryReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryReadModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CategoryReadModel>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == parentId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryReadModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
