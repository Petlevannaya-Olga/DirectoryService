using DirectoryService.Domain;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("departments_locations");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(x => x.Value, name => new DepartmentLocationsId(name));

        builder
            .Property(x => x.LocationId)
            .IsRequired()
            .HasColumnName("location_id")
            .HasConversion(x => x.Value, name => new LocationId(name));

        builder
            .Property(x => x.DepartmentId)
            .IsRequired()
            .HasColumnName("department_id")
            .HasConversion(x => x.Value, name => new DepartmentId(name));
    }
}