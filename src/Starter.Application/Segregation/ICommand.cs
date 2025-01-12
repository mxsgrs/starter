namespace Starter.Application.Segregation;

public interface ICommand { }

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    void Handle(TCommand command);
}

public interface ICommandAsync { }

public interface ICommandHandlerAsync<in TCommandAsync> where TCommandAsync : ICommandAsync
{
    Task HandleAsync(TCommandAsync command);
}
