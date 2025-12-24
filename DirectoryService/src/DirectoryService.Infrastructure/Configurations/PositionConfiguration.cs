using DirectoryService.Domain;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasColumnName("id");

        builder
            .Property(x => x.Name)
            .HasMaxLength(LengthConstants.LENGTH_100)
            .IsRequired()
            .HasConversion(x => x.Value, name => new PositionName(name))
            .HasColumnName("name");

        builder
            .Property(x => x.Description)
            .HasMaxLength(LengthConstants.LENGTH_1000)
            .IsRequired()
            .HasConversion(x => x.Value, name => new Description(name))
            .HasColumnName("description");

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