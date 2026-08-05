using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DirectoryService.Infrastructure;

public sealed class LocationsRepository(
    ApplicationDbContext dbContext,
    ILogger<LocationsRepository> logger)
    : ILocationsRepository
{
    public Task<Result<Guid, Error>> AddAsync(
        Location location,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<Result<Guid, Error>>(
                CommonErrors.OperationCancelled(
                    "add.location.was.canceled"));
        }

        dbContext.Locations.Add(location);

        return Task.FromResult<Result<Guid, Error>>(
            location.Id.Value);
    }

    public async Task<Result<Location, Error>> GetByAsync(
        Expression<Func<Location, bool>> expression,
        CancellationToken cancellationToken)
    {
        try
        {
            var location = await dbContext
                .Locations
                .FirstOrDefaultAsync(
                    expression,
                    cancellationToken);

            if (location is null)
            {
                return CommonErrors.NotFound(
                    "location.not.found",
                    "Локация не найдена");
            }

            return location;
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Операция получения локации была отменена");

            return CommonErrors.OperationCancelled(
                "get.location.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения локации");

            return CommonErrors.Db(
                "get.location.from.db.exception",
                "Ошибка получения локации");
        }
    }

    public async Task<Result<bool, Error>> ExistsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        try
        {
            var locationIds = ids
                .Distinct()
                .Select(id => new LocationId(id))
                .ToArray();

            var existingCount = await dbContext
                .Locations
                .CountAsync(
                    location => locationIds.Contains(location.Id),
                    cancellationToken);

            if (existingCount == locationIds.Length)
            {
                return true;
            }

            logger.LogWarning(
                "Некоторые локации отсутствуют в базе данных");

            return Errors.LocationsNotFound();
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Операция проверки существования локаций была отменена");

            return CommonErrors.OperationCancelled(
                "check.locations.exists.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка проверки существования локаций");

            return Errors.UnexpectedDbException();
        }
    }

    public async Task<Result<bool, Error>> ExistsAsync(
        Expression<Func<Location, bool>> expression,
        CancellationToken cancellationToken)
    {
        try
        {
            var exists = await dbContext
                .Locations
                .AnyAsync(
                    expression,
                    cancellationToken);

            return exists;
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Операция проверки существования локации была отменена");

            return CommonErrors.OperationCancelled(
                "check.location.exists.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка проверки существования локации");

            return CommonErrors.Db(
                "check.location.exists.in.db.exception",
                "Ошибка проверки существования локации");
        }
    }

    public async Task<Result<bool, Error>> ExistsAndActiveAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        try
        {
            var locationIds = ids
                .Distinct()
                .Select(id => new LocationId(id))
                .ToArray();

            var locations = await dbContext
                .Locations
                .Where(location => locationIds.Contains(location.Id))
                .Select(location => new
                {
                    location.Id,
                    location.IsActive
                })
                .ToListAsync(cancellationToken);

            if (locations.Count != locationIds.Length)
            {
                logger.LogWarning(
                    "Некоторые локации отсутствуют в базе данных");

                return Errors.LocationsNotFound();
            }

            if (locations.Any(location => !location.IsActive))
            {
                logger.LogWarning(
                    "Одна или несколько локаций неактивны");

                return Errors.LocationsInactive();
            }

            return true;
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Операция проверки существования и активности локаций была отменена");

            return CommonErrors.OperationCancelled(
                "check.locations.exists.and.active.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка проверки существования и активности локаций");

            return Errors.UnexpectedDbException();
        }
    }

    public async Task<UnitResult<Error>> EnsureExistsAndActiveAsync(
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var location = await dbContext
                .Locations
                .Where(item => item.Id == locationId)
                .Select(item => new
                {
                    item.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (location is null)
            {
                return Errors.LocationNotFound(
                    locationId.Value);
            }

            if (!location.IsActive)
            {
                return Errors.LocationInactive(
                    locationId.Value);
            }

            return UnitResult.Success<Error>();
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Операция проверки локации с id = {LocationId} была отменена",
                locationId.Value);

            return CommonErrors.OperationCancelled(
                "ensure.location.exists.and.active.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка проверки локации с id = {LocationId}",
                locationId.Value);

            return CommonErrors.Db(
                "ensure.location.exists.and.active.db.exception",
                $"Ошибка проверки локации с идентификатором '{locationId.Value}'");
        }
    }

    public async Task<UnitResult<Error>>
        EnsureExistsAndActiveForUpdateAsync(
            LocationId locationId,
            CancellationToken cancellationToken)
    {
        try
        {
            var location = await dbContext
                .Locations
                .FromSqlInterpolated(
                    $"""
                     SELECT *
                     FROM locations
                     WHERE id = {locationId.Value}
                     FOR UPDATE
                     """)
                .FirstOrDefaultAsync(cancellationToken);

            if (location is null)
            {
                return Errors.LocationNotFound(
                    locationId.Value);
            }

            if (!location.IsActive)
            {
                return Errors.LocationInactive(
                    locationId.Value);
            }

            return UnitResult.Success<Error>();
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Операция блокировки локации с id = {LocationId} была отменена",
                locationId.Value);

            return CommonErrors.OperationCancelled(
                "lock.location.for.update.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка блокировки локации с id = {LocationId}",
                locationId.Value);

            return CommonErrors.Db(
                "lock.location.for.update.db.exception",
                $"Ошибка блокировки локации с идентификатором '{locationId.Value}'");
        }
    }

    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error LocationsNotFound()
        {
            return CommonErrors.NotFound(
                "locations.not.found",
                "Некоторые заданные локации отсутствуют в базе данных");
        }

        public static Error LocationsInactive()
        {
            return CommonErrors.Validation(
                "locations.inactive",
                "Одна или несколько заданных локаций неактивны");
        }

        public static Error UnexpectedDbException()
        {
            return CommonErrors.Db(
                "check.locations.exists.db.exception",
                "Ошибка проверки существования локаций в базе данных");
        }

        public static Error LocationNotFound(Guid locationId)
        {
            return CommonErrors.NotFound(
                "location.not.found",
                $"Локация с идентификатором '{locationId}' не найдена",
                locationId);
        }

        public static Error LocationInactive(Guid locationId)
        {
            return CommonErrors.Validation(
                "location.inactive",
                $"Локация с идентификатором '{locationId}' неактивна",
                "locationId");
        }
    }
}