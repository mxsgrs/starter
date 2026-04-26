namespace Network.Application.Shared.Cqrs;

public interface IQuery<TResult> { }

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

public interface IQueryByIdHandler<TResult>
{
    Task<TResult> HandleAsync(Guid id, CancellationToken cancellationToken = default);
}