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
            .HasColumnName("id");

        builder
            .Property(x => x.Name)
            .HasMaxLength(LengthConstants.LENGTH_120)
            .IsRequired()
            .HasConversion(x => x.Value, name => new LocationName(name))
            .HasColumnName("name");

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
            .Property(x => x.Timezone)
            .IsRequired()
            .HasConversion(x => x.Value, name => new Timezone(name))
            .HasColumnName("timezone");

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
    }
}