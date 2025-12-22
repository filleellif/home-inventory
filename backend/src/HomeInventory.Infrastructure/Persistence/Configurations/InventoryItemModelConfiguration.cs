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

        // Location properties (flattened)
        builder.Property(x => x.RoomName)
            .HasColumnName("room_name")
            .HasMaxLength(100);

        builder.Property(x => x.RoomQrCode)
            .HasColumnName("room_qr_code")
            .HasMaxLength(100);

        builder.Property(x => x.ShelfName)
            .HasColumnName("shelf_name")
            .HasMaxLength(100);

        builder.Property(x => x.ShelfQrCode)
            .HasColumnName("shelf_qr_code")
            .HasMaxLength(100);

        builder.Property(x => x.BoxName)
            .HasColumnName("box_name")
            .HasMaxLength(100);

        builder.Property(x => x.BoxQrCode)
            .HasColumnName("box_qr_code")
            .HasMaxLength(100);

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
        builder.HasIndex(x => x.CategoryId)
            .HasDatabaseName("ix_inventory_items_category_id");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_inventory_items_created_at");

        // QR Code indexes (partial indexes for performance)
        builder.HasIndex(x => x.RoomQrCode)
            .HasDatabaseName("ix_inventory_items_room_qr_code")
            .HasFilter("room_qr_code IS NOT NULL");

        builder.HasIndex(x => x.ShelfQrCode)
            .HasDatabaseName("ix_inventory_items_shelf_qr_code")
            .HasFilter("shelf_qr_code IS NOT NULL");

        builder.HasIndex(x => x.BoxQrCode)
            .HasDatabaseName("ix_inventory_items_box_qr_code")
            .HasFilter("box_qr_code IS NOT NULL");
    }
}
