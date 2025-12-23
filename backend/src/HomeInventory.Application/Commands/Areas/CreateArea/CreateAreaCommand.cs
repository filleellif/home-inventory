namespace HomeInventory.Application.Commands.Areas.CreateArea;

public sealed record CreateAreaCommand(
    Guid Id,
    string Name,
    Guid? ParentAreaId,
    string? Description) : Command(Id);
