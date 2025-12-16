using AutoMapper;
using HomeInventory.Application.Common;
using HomeInventory.Application.DTOs;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Queries.Categories.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

            return Result<List<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            return Result<List<CategoryDto>>.Failure($"Failed to retrieve categories: {ex.Message}");
        }
    }
}
