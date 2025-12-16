using HomeInventory.Domain.Aggregates.CategoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value))
            .HasColumnName("id");

        // Properties
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(x => x.ParentCategoryId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? CategoryId.From(value.Value) : null)
            .HasColumnName("parent_category_id");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("ix_categories_name");

        builder.HasIndex(x => x.ParentCategoryId)
            .HasDatabaseName("ix_categories_parent_category_id");

        // Ignore domain events
        builder.Ignore(x => x.DomainEvents);
    }
}
