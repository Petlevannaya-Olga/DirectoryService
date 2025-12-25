using DirectoryService.Application;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Infrastructure;

public class LocationsEfCoreRepository(ApplicationDbContext dbContext) : ILocationsRepository
{
    public async Task<Guid> AddAsync(Location question, CancellationToken cancellationToken)
    {
        await dbContext.Locations.AddAsync(question, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return question.Id;
    }
}