namespace HomeInventory.Application.Commands;

public interface ICommandHandler<in TCommand> where TCommand : Command
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
