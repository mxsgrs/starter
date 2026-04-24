namespace UserService.Application.Shared.Cqrs;

public interface ICommand { }

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandByIdHandler
{
    Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken = default);
}


