using DirectoryService.Domain.Departments;
using DirectoryService.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(x => x.Value, name => new DepartmentId(name));

        builder.ComplexProperty(x => x.Name, config =>
        {
            config.Property(x => x.Value)
                .HasColumnName("name")
                .HasMaxLength(DepartmentName.MAXLENGTH)
                .IsRequired();
        });

        builder.Property(x => x.Slug)
            .HasConversion(
                v => v.Value,
                v => Slug.Create(v).Value)
            .HasColumnName("slug")
            .HasMaxLength(Slug.MAXLENGTH)
            .IsRequired();

        builder
            .HasIndex(x => x.Slug)
            .IsUnique();

        builder
            .Property(department => department.ParentId)
            .HasConversion<DepartmentIdConverter>()
            .HasColumnName("parent_id");

        builder.ComplexProperty(x => x.Path, config =>
        {
            config.Property(x => x.Value)
                .HasColumnName("path")
                .IsRequired();
        });

        builder
            .Property(x => x.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder
            .Property(x => x.ChildrenCount)
            .IsRequired()
            .HasColumnName("children_count");

        builder
            .Property(x => x.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder
            .Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder
            .Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder
            .HasMany(x => x.ChildrenDepartments)
            .WithOne()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.DepartmentLocations)
            .WithOne()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.DepartmentPositions)
            .WithOne()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}