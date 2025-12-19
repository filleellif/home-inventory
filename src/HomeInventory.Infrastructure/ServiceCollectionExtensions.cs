using HomeInventory.Domain.Repositories;
using HomeInventory.Infrastructure.Configuration;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Repositories;
using HomeInventory.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, PostgresOptions options)
    {
        Console.WriteLine(options.ConnectionString);
        
        // Database
        services.AddDbContext<ApplicationDbContext>(builder =>
            builder.UseNpgsql(
                options.ConnectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // File Storage
        // var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        // var baseUrl = configuration["FileStorage:BaseUrl"] ?? "http://localhost:5000";
        // services.AddSingleton<IFileStorageService>(new LocalFileStorageService(storagePath, baseUrl));

        return services;
    }
}