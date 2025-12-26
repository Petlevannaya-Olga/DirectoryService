using DirectoryService.Domain;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("departments_positions");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasConversion(x => x.Value, name => new DepartmentPositionsId(name));

        builder
            .Property(x => x.PositionId)
            .IsRequired()
            .HasColumnName("position_id")
            .HasConversion(x => x.Value, name => new PositionId(name));

        builder
            .Property(x => x.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id")
            .HasConversion(x => x.Value, name => new DepartmentId(name));
    }
}