using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HomeInventory.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<InventoryItemModel> InventoryItems => Set<InventoryItemModel>();

    public DbSet<CategoryModel> Categories => Set<CategoryModel>();

    public DbSet<ItemPhotoModel> ItemPhotos => Set<ItemPhotoModel>();

    public DbSet<ItemReceiptModel> ItemReceipts => Set<ItemReceiptModel>();

    public DbSet<ItemTagModel> ItemTags => Set<ItemTagModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}