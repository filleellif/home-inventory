using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Queries.Categories.GetAllCategories;

public class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetAllCategoriesQuery, Result<List<CategoryDto>>>
{
    public async Task<Result<List<CategoryDto>>> Handle(GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var categories = await categoryRepository.GetAllAsync(cancellationToken);

            var categoryDtos = categories
                .Select(c => c.FromDomain())
                .ToList();

            return Result<List<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            return Result<List<CategoryDto>>.Failure($"Failed to retrieve categories: {ex.Message}");
        }
    }
}