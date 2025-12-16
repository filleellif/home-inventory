using HomeInventory.Application.Common;
using MediatR;

namespace HomeInventory.Application.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentCategoryId
) : IRequest<Result<Guid>>;
