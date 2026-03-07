using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using Primitives;

namespace DirectoryService.Application;

public interface IPositionsRepository
{
    Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken);

    Task<Result<Position?, Error>> GetByAsync(Expression<Func<Position, bool>> expression, CancellationToken cancellationToken);
}