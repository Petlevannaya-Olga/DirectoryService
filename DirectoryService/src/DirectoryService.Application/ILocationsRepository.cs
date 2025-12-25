using DirectoryService.Domain.Locations;

namespace DirectoryService.Application;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location question, CancellationToken cancellationToken);
}