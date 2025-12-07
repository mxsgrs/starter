using MassTransit;

namespace UserService.Application.Events;

public record UserCreatedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
}

public interface IIntegrationEvent
{
    public DateTime OccurredOn { get; }
    public Guid Id { get; }
}

public abstract record IntegrationEvent : IIntegrationEvent
{
    public DateTime OccurredOn { get; } = DateTime.Now;
    public Guid Id { get; } = Guid.NewGuid();
}

public class UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger) : IConsumer<UserCreatedEvent>
{
    public Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received the user created event {UserCreatedEvent}", context.Message); 
        }

        return Task.CompletedTask;
    }
}
