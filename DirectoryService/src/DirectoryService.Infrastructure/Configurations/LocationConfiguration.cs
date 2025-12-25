using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            .Property(x => x.Address)
            .IsRequired()
            .HasColumnName("address")
            .HasColumnType("jsonb");

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