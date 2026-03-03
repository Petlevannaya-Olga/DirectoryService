using System.Diagnostics.CodeAnalysis;
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

    public async Task<Location?> GetByAddressAsync(Address address, CancellationToken cancellationToken)
    {
        return await dbContext.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                l =>
                    l.Address.PostalCode == address.PostalCode &&
                    l.Address.City == address.City &&
                    l.Address.Region == address.Region &&
                    l.Address.Street == address.Street &&
                    l.Address.House == address.House &&
                    l.Address.Apartment == address.Apartment,
                cancellationToken);
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
    }
}