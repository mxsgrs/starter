namespace UserService.Application.Shared.Cqrs;

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


