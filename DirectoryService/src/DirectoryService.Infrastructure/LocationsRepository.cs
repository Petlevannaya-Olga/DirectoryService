using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Locations;
using Primitives;

namespace DirectoryService.Infrastructure;

public class LocationsRepository(ApplicationDbContext dbContext) : ILocationsRepository
{
    public async Task<Result<Guid, Error>> AddAsync(Location question, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.Locations.AddAsync(question, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return question.Id.Value;
        }
        catch (Exception e)
        {
            return CommonErrors.Db("add.location.to.db.exception", e.Message);
        }
    }
}