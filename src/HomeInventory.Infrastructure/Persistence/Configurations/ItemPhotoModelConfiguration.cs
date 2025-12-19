using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class ItemPhotoModelConfiguration : IEntityTypeConfiguration<ItemPhotoModel>
{
    public void Configure(EntityTypeBuilder<ItemPhotoModel> builder)
    {
        builder.ToTable("item_photos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.ItemId)
            .HasColumnName("item_id")
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.FileUrl)
            .HasColumnName("file_url")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.MediaType)
            .HasColumnName("media_type")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.UploadedAt)
            .HasColumnName("uploaded_at")
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .HasColumnName("file_size_bytes")
            .IsRequired();
    }
}
