namespace HomeInventory.Application.Commands.Areas.UpdateArea;

public sealed record UpdateAreaCommand(
    Guid Id,
    string Name,
    Guid? ParentAreaId,
    string? Description) : Command(Id);
