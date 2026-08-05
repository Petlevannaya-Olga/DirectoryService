namespace DirectoryService.Infrastructure.Database;

internal static class DatabaseConstraintNames
{
    public const string DepartmentName =
        "ix_departments_name";

    public const string DepartmentSlug =
        "ix_departments_slug";

    public const string LocationName =
        "ix_locations_name";

    public const string LocationAddress =
        "ix_locations_address";

    public const string PositionName =
        "ix_positions_name";
}