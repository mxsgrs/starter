namespace UserService.Application.Cqrs;

#region Command

public interface ICommand { }

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandByIdHandler
{
    Task HandleAsync(Guid id, CancellationToken cancellationToken = default);
}

#endregion

#region Query

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

#endregion
