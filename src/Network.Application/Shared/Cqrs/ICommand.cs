namespace Network.Application.Shared.Cqrs;

public interface ICommand { }

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command);
}

public interface ICommandByIdHandler
{
    Task<Result> HandleAsync(Guid id);
}

public interface ICommandHandlerResultingGuid<TCommand>
    where TCommand : ICommand
{
    Task<Result<Guid>> HandleAsync(TCommand command);
}
