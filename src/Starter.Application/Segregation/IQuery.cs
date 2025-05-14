namespace Starter.Application.Segregation;

public interface IQuery<out TResult> { }

public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
{
    TResult Handle(TQuery query);
}

public interface IQueryAsync<out TResult> { }

public interface IQueryHandlerAsync<in TQueryAsync, TResult> where TQueryAsync : IQueryAsync<TResult>
{
    Task<TResult> HandleAsync(TQueryAsync query);
}