namespace HomeInventory.Application.Commands.Items.CreateItem;

public sealed record CreateItemCommand(
    Guid Id,
    string Name,
    string? Description,
    int Quantity,
    Guid? AreaId,
    Guid? CategoryId
) : Command(Id);