using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Repositories;

namespace HomeInventory.Application.Commands.Categories.CreateCategory;

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategoryCommand>
{
    public async Task HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var parentCategoryId = command.ParentCategoryId.HasValue
            ? CategoryId.From(command.ParentCategoryId.Value)
            : null;

        var category = Category.Create(
            CategoryId.From(command.Id),
            command.Name,
            command.Description,
            parentCategoryId
        );

        await categoryRepository.AddAsync(category, cancellationToken);
    }
}