using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class InventoryItemModelConfiguration : IEntityTypeConfiguration<InventoryItemModel>
{
    public void Configure(EntityTypeBuilder<InventoryItemModel> builder)
    {
        builder.ToTable("inventory_items");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        // BasicInfo properties (flattened)
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        // FinancialInfo properties (flattened)
        builder.Property(x => x.PurchasePriceAmount)
            .HasColumnName("purchase_price_amount")
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PurchasePriceCurrency)
            .HasColumnName("purchase_price_currency")
            .HasMaxLength(3);

        builder.Property(x => x.CurrentValueAmount)
            .HasColumnName("current_value_amount")
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CurrentValueCurrency)
            .HasColumnName("current_value_currency")
            .HasMaxLength(3);

        builder.Property(x => x.PurchaseDate)
            .HasColumnName("purchase_date");

        // AreaId
        builder.Property(x => x.AreaId)
            .HasColumnName("area_id");

        // CategoryId
        builder.Property(x => x.CategoryId)
            .HasColumnName("category_id");

        // Timestamps
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Photos (HasMany relationship)
        builder.HasMany(x => x.Photos)
            .WithOne()
            .HasForeignKey(p => p.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Receipts (HasMany relationship)
        builder.HasMany(x => x.Receipts)
            .WithOne()
            .HasForeignKey(r => r.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.AreaId)
            .HasDatabaseName("ix_inventory_items_area_id");

        builder.HasIndex(x => x.CategoryId)
            .HasDatabaseName("ix_inventory_items_category_id");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_inventory_items_created_at");
    }
}