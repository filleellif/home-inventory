namespace HomeInventory.Application.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentCategoryId
) : Command(Id);