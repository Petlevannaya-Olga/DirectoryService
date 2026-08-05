using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public sealed class PositionConfiguration
    : IEntityTypeConfiguration<Position>
{
    private const string ACTIVE_NAME_INDEX =
        "ux_positions_active_name";

    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(position => position.Id);

        builder
            .Property(position => position.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new PositionId(value));

        builder
            .Property(position => position.Name)
            .HasColumnName("name")
            .HasMaxLength(PositionName.MAXLENGTH)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                value => PositionName.Create(value).Value);

        builder
            .HasIndex(position => position.Name)
            .IsUnique()
            .HasDatabaseName(ACTIVE_NAME_INDEX)
            .HasFilter("is_active = true");

        builder
            .ComplexProperty(
                position => position.Description,
                descriptionBuilder =>
                {
                    descriptionBuilder
                        .Property(description => description.Value)
                        .HasColumnName("description")
                        .HasMaxLength(Description.MAXLENGTH)
                        .IsRequired();
                });

        builder
            .Property(position => position.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder
            .Property(position => position.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder
            .Property(position => position.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}