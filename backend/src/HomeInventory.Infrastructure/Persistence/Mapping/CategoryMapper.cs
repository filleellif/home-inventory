using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

public static class CategoryMapper
{
    public static Category ToDomain(this CategoryModel model)
    {
        var categoryId = CategoryId.From(model.Id);
        var parentCategoryId = model.ParentCategoryId.HasValue
            ? CategoryId.From(model.ParentCategoryId.Value)
            : null;

        var category = Category.Create(
            categoryId,
            model.Name,
            model.Description,
            parentCategoryId
        );

        // Use reflection to set timestamps since they're set in the constructor
        typeof(Category)
            .GetProperty(nameof(Category.CreatedAt))!
            .SetValue(category, model.CreatedAt);

        typeof(Category)
            .GetProperty(nameof(Category.UpdatedAt))!
            .SetValue(category, model.UpdatedAt);

        return category;
    }

    public static CategoryModel ToModel(this Category domain)
    {
        return new CategoryModel
        {
            Id = domain.Id.Value,
            Name = domain.Name,
            Description = domain.Description,
            ParentCategoryId = domain.ParentCategoryId?.Value,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}
