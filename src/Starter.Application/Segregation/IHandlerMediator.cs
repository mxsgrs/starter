namespace Starter.Application.Segregation;

public interface IHandlerMediator
{
    TResult Handle<TQuery, TResult>(TQuery query) 
        where TQuery : IQuery<TResult>;

    void Handle<TCommand>(TCommand command) 
        where TCommand : ICommand;

    Task<TResult> HandleAsync<TQueryAsync, TResult>(TQueryAsync query)
        where TQueryAsync : IQueryAsync<TResult>;
    Task HandleAsync<TCommandAsync>(TCommandAsync query)
        where TCommandAsync : ICommandAsync;
}
