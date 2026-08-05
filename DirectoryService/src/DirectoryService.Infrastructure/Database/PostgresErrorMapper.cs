using Microsoft.EntityFrameworkCore;
using Npgsql;
using Primitives;

namespace DirectoryService.Infrastructure.Database;

internal static class PostgresErrorMapper
{
    public static Error Map(DbUpdateException exception)
    {
        if (exception.InnerException is not PostgresException postgresException)
        {
            return UnexpectedUpdateError();
        }

        return postgresException.SqlState switch
        {
            PostgresErrorCodes.UniqueViolation =>
                MapUniqueViolation(postgresException),

            _ => UnexpectedUpdateError()
        };
    }

    private static Error MapUniqueViolation(
        PostgresException exception)
    {
        return exception.ConstraintName switch
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

            DatabaseConstraintNames.PositionName =>
                CommonErrors.Conflict(
                    "position.name.conflict",
                    "Позиция с таким названием уже существует"),

            _ => CommonErrors.Conflict(
                "db.unique.constraint.conflict",
                "Запись с такими данными уже существует")
        };
    }

    private static Error UnexpectedUpdateError()
    {
        return CommonErrors.Db(
            "db.update.failed",
            "Не удалось сохранить изменения в базе данных");
    }
}