using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Primitives;

namespace DirectoryService.Application;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByAsync(
        Expression<Func<Location, bool>> expression,
        CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsAndActiveAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<UnitResult<Error>> EnsureExistsAndActiveAsync(
        LocationId id,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> EnsureExistsAndActiveForUpdateAsync(
        LocationId locationId,
        CancellationToken cancellationToken);
}