using DirectoryService.Domain;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Primitives;
using Path = DirectoryService.Domain.Departments.Path;

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
                .HasMaxLength(LengthConstants.LENGTH_150)
                .IsRequired();
        });


        builder.ComplexProperty(x => x.Identifier, config =>
        {
            config.Property(x => x.Value)
                .HasColumnName("identifier")
                .HasMaxLength(LengthConstants.LENGTH_150)
                .IsRequired();
        });

        builder
            .Property(x => x.ParentId)
            .IsRequired(false)
            .HasConversion(x => x!.Value, name => new DepartmentId(name))
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
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.DepartmentPositions)
            .WithOne()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}