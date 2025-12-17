using HomeInventory.Application.DTOs;
using HomeInventory.Domain.Aggregates.CategoryAggregate;

namespace HomeInventory.Application.Mapping;

public static class CategoryMappingExtensions
{
    public static CategoryDto FromDomain(this Category category) => new()
    {
        Id = category.Id.Value,
        Name = category.Name,
        Description = category.Description,
        ParentCategoryId = category.ParentCategoryId?.Value,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt,
    };
}