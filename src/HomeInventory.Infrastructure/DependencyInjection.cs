using HomeInventory.Application.Interfaces;
using HomeInventory.Domain.Repositories;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Repositories;
using HomeInventory.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // File Storage
        var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var baseUrl = configuration["FileStorage:BaseUrl"] ?? "http://localhost:5000";
        services.AddSingleton<IFileStorageService>(new LocalFileStorageService(storagePath, baseUrl));

        return services;
    }
}
