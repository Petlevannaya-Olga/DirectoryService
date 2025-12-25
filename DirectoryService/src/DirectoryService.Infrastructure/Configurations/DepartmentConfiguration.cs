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
            .HasColumnName("id");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(LengthConstants.LENGTH_150)
            .HasConversion(x => x.Value, name => new DepartmentName(name))
            .HasColumnName("name");

        builder
            .Property(x => x.Identifier)
            .IsRequired()
            .HasMaxLength(LengthConstants.LENGTH_150)
            .HasConversion(x => x.Value, name => new Identifier(name))
            .HasColumnName("identifier");

        builder
            .Property(x => x.ParentId)
            .IsRequired(false)
            .HasColumnName("parent_id");

        builder
            .Property(x => x.Path)
            .IsRequired()
            .HasConversion(x => x.Value, name => new Path(name))
            .HasColumnName("path");

        builder
            .Property(x => x.Depth)
            .IsRequired()
            .HasColumnName("depth");

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
            .HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}