using DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("departments_locations");

        builder.HasKey(x => new { x.DepartmentId, x.LocationId });

        builder
            .Property(x => x.LocationId)
            .IsRequired()
            .HasColumnName("location_id");

        builder
            .Property(x => x.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id");

        builder.HasOne(x => x.Location)
            .WithMany(x => x.Departments)
            .HasForeignKey(x => x.LocationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}