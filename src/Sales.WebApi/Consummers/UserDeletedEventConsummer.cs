using MassTransit;

// Duplicate of Network.Application.IntegrationEvents.UserDeletedIntegrationEvent.
// Namespace must match the publisher's namespace for MassTransit routing.
namespace Network.Application.Users.Events;

public record UserDeletedIntegrationEvent(Guid UserId);

public class UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger)
    : IConsumer<UserDeletedIntegrationEvent>
{
    /// <summary>
    /// Handle the user deleted integration event
    /// </summary>
    public Task Consume(ConsumeContext<UserDeletedIntegrationEvent> context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received the user deleted event {UserDeletedIntegrationEvent}", context.Message);
        }

        return Task.CompletedTask;
    }
}
