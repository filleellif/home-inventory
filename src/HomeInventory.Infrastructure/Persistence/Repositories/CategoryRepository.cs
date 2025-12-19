using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Repositories;
using HomeInventory.Infrastructure.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        var model = await context.Categories
            .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

        return model?.ToDomain();
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var models = await context.Categories
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var model = await context.Categories
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return model?.ToDomain();
    }

    public async Task<List<Category>> GetChildCategoriesAsync(CategoryId parentId, CancellationToken cancellationToken = default)
    {
        var models = await context.Categories
            .Where(x => x.ParentCategoryId == parentId.Value)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return models.Select(m => m.ToDomain()).ToList();
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        var model = category.ToModel();
        await context.Categories.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var model = category.ToModel();
        context.Categories.Update(model);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        var model = await context.Categories
            .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

        if (model != null)
        {
            context.Categories.Remove(model);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
