using MassTransit;

// Duplicate of Network.Application.IntegrationEvents.UserCreatedIntegrationEvent.
// Namespace must match the publisher's namespace for MassTransit routing.
namespace Network.Application.Users.Events;

public record UserCreatedIntegrationEvent(Guid UserId);

public class UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger)
    : IConsumer<UserCreatedIntegrationEvent>
{
    /// <summary>
    /// Handle the user created integration event
    /// </summary>
    public Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received the user created event {UserCreatedIntegrationEvent}", context.Message);
        }

        return Task.CompletedTask;
    }
}
