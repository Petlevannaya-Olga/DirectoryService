using DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("departments_positions");

        builder.HasKey(x => new { x.DepartmentId, x.PositionId });

        builder
            .Property(x => x.PositionId)
            .IsRequired()
            .HasColumnName("position_id");

        builder
            .Property(x => x.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id");

        builder.HasOne(x => x.Position)
            .WithMany(x => x.Departments)
            .HasForeignKey(x => x.PositionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}