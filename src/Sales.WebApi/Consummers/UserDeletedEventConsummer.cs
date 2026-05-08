using MassTransit;
using Sales.WebApi.Persistence.Repositories;

// Duplicate of Network.Application.IntegrationEvents.UserDeletedIntegrationEvent.
// Namespace must match the publisher's namespace for MassTransit routing.
namespace Network.Application.Users.Events;

public record UserDeletedIntegrationEvent(Guid UserId);

public class UserDeletedEventConsumer(ILogger<UserDeletedEventConsumer> logger, IUserRepository userRepository)
    : IConsumer<UserDeletedIntegrationEvent>
{
    /// <summary>
    /// Handle the user deleted integration event
    /// </summary>
    public async Task Consume(ConsumeContext<UserDeletedIntegrationEvent> context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received the user deleted event {UserDeletedIntegrationEvent}", context.Message);
        }

        await userRepository.DeleteAsync(context.Message.UserId);
    }
}
