using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => ItemId.From(value))
            .HasColumnName("id");

        // BasicInfo (Owned Value Object)
        builder.OwnsOne(x => x.BasicInfo, basicInfo =>
        {
            basicInfo.Property(b => b.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            basicInfo.Property(b => b.Description)
                .HasColumnName("description")
                .HasMaxLength(2000);

            basicInfo.Property(b => b.Quantity)
                .HasColumnName("quantity")
                .IsRequired();
        });

        // FinancialInfo (Owned Value Object)
        builder.OwnsOne(x => x.FinancialInfo, financialInfo =>
        {
            financialInfo.OwnsOne(f => f.PurchasePrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("purchase_price_amount")
                    .HasColumnType("decimal(18,2)");

                money.Property(m => m.Currency)
                    .HasColumnName("purchase_price_currency")
                    .HasMaxLength(3);
            });

            financialInfo.OwnsOne(f => f.CurrentValue, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("current_value_amount")
                    .HasColumnType("decimal(18,2)");

                money.Property(m => m.Currency)
                    .HasColumnName("current_value_currency")
                    .HasMaxLength(3);
            });

            financialInfo.Property(f => f.PurchaseDate)
                .HasColumnName("purchase_date");
        });

        // Location (Owned Value Object)
        builder.OwnsOne(x => x.Location, location =>
        {
            location.Property(l => l.Room)
                .HasColumnName("room")
                .HasMaxLength(100);

            location.Property(l => l.StorageSpot)
                .HasColumnName("storage_spot")
                .HasMaxLength(100);

            location.OwnsOne(l => l.Coordinates, coords =>
            {
                coords.Property(c => c.Latitude)
                    .HasColumnName("gps_latitude");

                coords.Property(c => c.Longitude)
                    .HasColumnName("gps_longitude");
            });
        });

        // CategoryId (reference to Category aggregate)
        builder.Property(x => x.CategoryId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? CategoryId.From(value.Value) : null)
            .HasColumnName("category_id");

        // Photos (Owned Collection)
        builder.OwnsMany(x => x.Photos, photos =>
        {
            photos.ToTable("item_photos");
            photos.WithOwner().HasForeignKey("item_id");
            photos.Property<Guid>("Id").HasColumnName("id");
            photos.HasKey("Id");

            photos.Property(p => p.FileName)
                .HasColumnName("file_name")
                .HasMaxLength(255)
                .IsRequired();

            photos.Property(p => p.FileUrl)
                .HasColumnName("file_url")
                .HasMaxLength(500)
                .IsRequired();

            photos.Property(p => p.MediaType)
                .HasColumnName("media_type")
                .HasMaxLength(20)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<MediaType>(v))
                .IsRequired();

            photos.Property(p => p.UploadedAt)
                .HasColumnName("uploaded_at")
                .IsRequired();

            photos.Property(p => p.FileSizeBytes)
                .HasColumnName("file_size_bytes")
                .IsRequired();
        });

        // Receipts (Owned Collection)
        builder.OwnsMany(x => x.Receipts, receipts =>
        {
            receipts.ToTable("item_receipts");
            receipts.WithOwner().HasForeignKey("item_id");
            receipts.Property<Guid>("Id").HasColumnName("id");
            receipts.HasKey("Id");

            receipts.Property(r => r.FileName)
                .HasColumnName("file_name")
                .HasMaxLength(255)
                .IsRequired();

            receipts.Property(r => r.FileUrl)
                .HasColumnName("file_url")
                .HasMaxLength(500)
                .IsRequired();

            receipts.Property(r => r.MediaType)
                .HasColumnName("media_type")
                .HasMaxLength(20)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<MediaType>(v))
                .IsRequired();

            receipts.Property(r => r.UploadedAt)
                .HasColumnName("uploaded_at")
                .IsRequired();

            receipts.Property(r => r.FileSizeBytes)
                .HasColumnName("file_size_bytes")
                .IsRequired();
        });

        // Tags (Owned Collection)
        builder.OwnsMany(x => x.Tags, tags =>
        {
            tags.ToTable("item_tags");
            tags.WithOwner().HasForeignKey("item_id");

            tags.Property(t => t.Value)
                .HasColumnName("tag_value")
                .HasMaxLength(50)
                .IsRequired();

            tags.HasKey("item_id", nameof(HomeInventory.Domain.ValueObjects.Tag.Value));
        });

        // Timestamps
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.CategoryId).HasDatabaseName("ix_inventory_items_category_id");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_inventory_items_created_at");

        // Ignore domain events
        builder.Ignore(x => x.DomainEvents);
    }
}
