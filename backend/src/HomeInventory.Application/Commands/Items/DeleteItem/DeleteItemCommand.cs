namespace HomeInventory.Application.Commands.Items.DeleteItem;

public record DeleteItemCommand(Guid Id) : Command(Id);
