using Microsoft.EntityFrameworkCore;
using Npgsql;
using Primitives;

namespace DirectoryService.Infrastructure.Database;

internal static class PostgresErrorMapper
{
    public static bool TryMap(
        DbUpdateException exception,
        out Error error)
    {
        if (exception.InnerException is not PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: not null
            } postgresException)
        {
            error = default!;
            return false;
        }

        var mappedError =
            postgresException.ConstraintName switch
            {
                DatabaseConstraintNames.DepartmentName =>
                    CommonErrors.Conflict(
                        "department.name.conflict",
                        "Подразделение с таким названием уже существует"),

                DatabaseConstraintNames.DepartmentSlug =>
                    CommonErrors.Conflict(
                        "department.slug.conflict",
                        "Подразделение с таким идентификатором уже существует"),

                DatabaseConstraintNames.LocationName =>
                    CommonErrors.Conflict(
                        "location.name.conflict",
                        "Локация с таким названием уже существует"),

                DatabaseConstraintNames.LocationAddress =>
                    CommonErrors.Conflict(
                        "location.address.conflict",
                        "Локация с таким адресом уже существует"),

                DatabaseConstraintNames.PositionActiveName =>
                    CommonErrors.Conflict(
                        "position.name.conflict",
                        "Активная позиция с таким названием уже существует"),

                _ => null
            };

        if (mappedError is null)
        {
            error = CommonErrors.Conflict(
                "db.unique.constraint.conflict",
                "Запись с такими данными уже существует");

            return true;
        }

        error = mappedError;
        return true;
    }
}