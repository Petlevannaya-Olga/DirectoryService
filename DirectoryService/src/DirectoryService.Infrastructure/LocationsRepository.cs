using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Primitives;

namespace DirectoryService.Infrastructure;

public class LocationsRepository(
    ApplicationDbContext dbContext,
    ILogger<LocationsRepository> logger) : ILocationsRepository
{
    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await dbContext.Locations.AddAsync(location, cancellationToken);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return location.Id.Value;
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException postgresException)
        {
            if (postgresException is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null }
                && postgresException.ConstraintName.Contains(
                    "locations_name",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return Errors.LocationNameConflict(location.Name.Value);
            }

            logger.LogError(e, "Ошибка добавления новой локации '{LocationName}'", location.Name.Value);
            return CommonErrors.Db(
                "add.location.to.db.exception",
                $"Ошибка добавления новой локации '{location.Name.Value}'");
        }
        catch (OperationCanceledException e)
        {
            logger.LogError(e, "Операция создания локации '{LocationName}' была отменена", location.Name.Value);
            return CommonErrors.OperationCancelled("add.location.was.canceled");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка добавления новой локации '{LocationName}'", location.Name.Value);
            return CommonErrors.Db(
                "add.location.to.db.exception",
                $"Ошибка добавления новой локации '{location.Name.Value}'");
        }
    }

    public async Task<Result<Location?, Error>> GetByAsync(
        Expression<Func<Location, bool>> expression,
        CancellationToken cancellationToken)
    {
        try
        {
            var location = await dbContext
                .Locations
                .FirstOrDefaultAsync(expression, cancellationToken);

            return location;
        }
        catch (OperationCanceledException)
        {
            logger.LogError("Операция получения локации была отменена");
            return CommonErrors.OperationCancelled("get.location.was.canceled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении локации");
            return CommonErrors.Db(
                "get.location.from.db.exception",
                $"Ошибка при получении локации");
        }
    }

    public async Task<Result<bool, Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        try
        {
            var locationIds = ids.Select(x => new LocationId(x)).ToList();

            int existingCount = await dbContext
                .Locations
                .CountAsync(l => locationIds.Contains(l.Id), cancellationToken);

            if (existingCount == locationIds.Count)
            {
                return true;
            }

            logger.LogError("Некоторые локации отсутствуют в БД");
            return Errors.LocationsNotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Произошла непредвиденная ошибка в процессе проверки наличия локаций в БД");

            return Errors.UnexpectedDbException();
        }
    }

    public async Task<Result<bool, Error>> ExistsAndActiveAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        try
        {
            var locationIds = ids
                .Select(id => new LocationId(id))
                .Distinct()
                .ToList();

            bool invalidLocationExists = await dbContext.Locations
                .Where(location => locationIds.Contains(location.Id))
                .AnyAsync(
                    location => !location.IsActive,
                    cancellationToken);

            if (invalidLocationExists)
            {
                logger.LogError("Одна или несколько локаций неактивны");
                return Errors.LocationsInactive();
            }

            int existingCount = await dbContext.Locations
                .CountAsync(
                    location => locationIds.Contains(location.Id),
                    cancellationToken);

            if (existingCount == locationIds.Count)
            {
                return true;
            }

            logger.LogError("Некоторые локации отсутствуют в БД");
            return Errors.LocationsNotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Произошла непредвиденная ошибка в процессе проверки наличия и активности локаций в БД");

            return Errors.UnexpectedDbException();
        }
    }

    public async Task<UnitResult<Error>> EnsureExistsAndActiveAsync(
        LocationId id,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext.Locations
            .AnyAsync(
                location =>
                    location.Id == id &&
                    location.IsActive,
                cancellationToken);

        if (!exists)
        {
            return Errors.LocationNotFound(id.Value);
        }

        return UnitResult.Success<Error>();
    }

    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error LocationNameConflict(string locationName)
        {
            return CommonErrors.Conflict(
                "location.name.conflict",
                $"Локация с заголовком {locationName} уже существует");
        }

        public static Error LocationsNotFound()
        {
            return CommonErrors.NotFound(
                "locations.not.found",
                $"Некоторые заданные локации отсутствуют в базе данных", null);
        }

        public static Error LocationsInactive()
        {
            return CommonErrors.Validation(
                "locations.inactive",
                $"Одна или несколько заданных локаций неактивны", null);
        }

        public static Error UnexpectedDbException()
        {
            return CommonErrors.Db(
                "get.exists.from.db.exception",
                $"Произошла непредвиденная ошибка в процессе проверки существования локаций в БД");
        }

        public static Error LocationNotFound(Guid id)
        {
            return new Error(
                "location.not.found",
                $"Локация с идентификатором '{id}' не найдена",
                ErrorType.NOT_FOUND);
        }
    }
}