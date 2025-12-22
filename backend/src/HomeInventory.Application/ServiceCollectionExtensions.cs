using FluentValidation;
using HomeInventory.Application.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HomeInventory.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Register command handlers with validation decorator
        RegisterCommandHandlers(services);

        return services;
    }

    private static void RegisterCommandHandlers(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Find all ICommandHandler<,> implementations
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .Select(i => new { Interface = i, Implementation = t }))
            .ToList();

        foreach (var handler in handlerTypes)
        {
            var commandType = handler.Interface.GetGenericArguments()[0];

            // Register the actual handler implementation
            services.AddScoped(handler.Implementation);

            // Register with decorator for validation
            services.AddScoped(handler.Interface, provider =>
            {
                var innerHandler = provider.GetRequiredService(handler.Implementation);

                var validatorType = typeof(IValidator<>).MakeGenericType(commandType);
                var validators = provider.GetServices(validatorType);

                var decoratorType = typeof(ValidatingCommandHandler<>)
                    .MakeGenericType(commandType);

                return Activator.CreateInstance(decoratorType, innerHandler, validators)!;
            });
        }
    }
}
