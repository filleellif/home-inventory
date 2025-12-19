using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class ItemTagModelConfiguration : IEntityTypeConfiguration<ItemTagModel>
{
    public void Configure(EntityTypeBuilder<ItemTagModel> builder)
    {
        builder.ToTable("item_tags");

        // Composite key
        builder.HasKey(x => new { x.ItemId, x.TagValue });

        builder.Property(x => x.ItemId)
            .HasColumnName("item_id")
            .IsRequired();

        builder.Property(x => x.TagValue)
            .HasColumnName("tag_value")
            .HasMaxLength(50)
            .IsRequired();
    }
}
