using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Primitives;

namespace DirectoryService.Application;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location question, CancellationToken cancellationToken);
}