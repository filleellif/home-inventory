using HomeInventory.Queries.Implementations;
using HomeInventory.Queries.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Queries;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<IItemQueries, ItemQueries>();
        services.AddScoped<ICategoryQueries, CategoryQueries>();
        services.AddScoped<IAreaQueries, AreaQueries>();

        return services;
    }
}