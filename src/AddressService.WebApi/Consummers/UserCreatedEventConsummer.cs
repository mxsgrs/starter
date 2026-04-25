using MassTransit;

// Keep this namespace intact otherwise MassTransit will break.
namespace UserService.Domain.Aggregates.UserAggregate;

public record UserCreatedDomainEvent(Guid UserId);

public class UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger) : IConsumer<UserCreatedDomainEvent>
{
    public Task Consume(ConsumeContext<UserCreatedDomainEvent> context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received the user created event {UserCreatedDomainEvent}", context.Message);
        }

        return Task.CompletedTask;
    }
}
