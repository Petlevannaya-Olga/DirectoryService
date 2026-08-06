using CSharpFunctionalExtensions;

namespace Primitives.Abstractions;

public interface IQueryHandler<TResponse, in TQuery>
    where TQuery : IQuery
{
    Task<Result<TResponse, Errors>> Handle(
        TQuery query,
        CancellationToken cancellationToken = default);
}