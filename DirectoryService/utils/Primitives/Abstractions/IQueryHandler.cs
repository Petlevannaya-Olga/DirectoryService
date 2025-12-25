namespace Primitives.Abstractions;

public interface IQueryHandler<TResponse, in TQuery>
    where TQuery : IQuery
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default);
}

public interface IQuery;