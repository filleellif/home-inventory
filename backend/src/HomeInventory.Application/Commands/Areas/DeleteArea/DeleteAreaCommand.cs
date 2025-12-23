namespace HomeInventory.Application.Commands.Areas.DeleteArea;

public sealed record DeleteAreaCommand(Guid Id) : Command(Id);
