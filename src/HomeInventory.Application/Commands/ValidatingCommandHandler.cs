using FluentValidation;

namespace HomeInventory.Application.Commands;

public class ValidatingCommandHandler<TCommand>(
    ICommandHandler<TCommand> innerHandler,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand>
    where TCommand : Command
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TCommand>(command);

            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        await innerHandler.HandleAsync(command, cancellationToken);
    }
}