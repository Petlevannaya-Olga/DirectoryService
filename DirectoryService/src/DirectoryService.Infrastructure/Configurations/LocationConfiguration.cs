using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Primitives;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(x => x.Value, name => new LocationId(name));

        builder
            .ComplexProperty(x => x.Name, config =>
            {
                config.Property(x => x.Value)
                    .HasMaxLength(LengthConstants.LENGTH_120)
                    .IsRequired()
                    .HasColumnName("name");
            });

        builder
            .OwnsOne(x => x.Address, b =>
            {
                b.ToJson("address");

                b.Property(x => x.PostalCode).IsRequired();
                b.Property(x => x.Region).IsRequired();
                b.Property(x => x.City).IsRequired();
                b.Property(x => x.Street).IsRequired();
                b.Property(x => x.House).IsRequired();
            });

        builder
            .ComplexProperty(x => x.Timezone, config =>
            {
                config.Property(x => x.Value)
                    .HasMaxLength(LengthConstants.LENGTH_120)
                    .IsRequired()
                    .HasColumnName("timezone");
            });

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
            .HasMany(x => x.DepartmentLocations)
            .WithOne()
            .HasForeignKey(x => x.LocationId);
    }
}