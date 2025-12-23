using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class AreaModelConfiguration : IEntityTypeConfiguration<AreaModel>
{
    public void Configure(EntityTypeBuilder<AreaModel> builder)
    {
        builder.ToTable("areas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(x => x.ParentAreaId)
            .HasColumnName("parent_area_id");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Self-referencing relationship
        builder.HasOne(x => x.ParentArea)
            .WithMany(x => x.ChildAreas)
            .HasForeignKey(x => x.ParentAreaId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        // Index on name for searching
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("ix_areas_name");

        // Index on parent for hierarchy queries
        builder.HasIndex(x => x.ParentAreaId)
            .HasDatabaseName("ix_areas_parent_area_id");
    }
}
