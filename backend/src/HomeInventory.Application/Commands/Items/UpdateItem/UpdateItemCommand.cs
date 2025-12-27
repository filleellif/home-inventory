namespace HomeInventory.Application.Commands.Items.UpdateItem;

public record UpdateItemCommand(
    Guid Id,
    string Name,
    string? Description,
    int Quantity,
    Guid? AreaId,
    Guid? CategoryId
) : Command(Id);
