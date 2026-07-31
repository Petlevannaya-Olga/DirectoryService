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
            .HasColumnName("id")
            .HasConversion(x => x.Value, name => new PositionId(name));

        builder
            .ComplexProperty(x => x.Name, config =>
            {
                config.Property(x => x.Value)
                    .HasMaxLength(PositionName.MAXLENGTH)
                    .IsRequired()
                    .HasColumnName("name");
            });

        builder
            .ComplexProperty(x => x.Description, config =>
            {
                config.Property(x => x.Value)
                    .HasMaxLength(Description.MAXLENGTH)
                    .IsRequired()
                    .HasColumnName("description");
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
    }
}