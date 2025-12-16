using HomeInventory.Application.Common;
using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Repositories;
using MediatR;

namespace HomeInventory.Application.Commands.Categories.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var parentCategoryId = request.ParentCategoryId.HasValue
                ? CategoryId.From(request.ParentCategoryId.Value)
                : null;

            var category = Category.Create(
                CategoryId.New(),
                request.Name,
                request.Description,
                parentCategoryId
            );

            await _categoryRepository.AddAsync(category, cancellationToken);

            return Result<Guid>.Success(category.Id.Value);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create category: {ex.Message}");
        }
    }
}
