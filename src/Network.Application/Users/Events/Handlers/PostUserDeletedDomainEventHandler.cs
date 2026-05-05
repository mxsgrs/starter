using Network.Application.Shared.Events;
using Network.Application.Shared.Interfaces;

namespace Network.Application.Users.Events.Handlers;

public class PostUserDeletedDomainEventHandler(IIntegrationEventPublisher publisher)
    : IPostSavedDomainEventHandler<UserDeletedDomainEvent>
{
    /// <summary>
    /// Publish an integration event to the message bus after the user has been deleted from the database
    /// </summary>
    public async Task PublishAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
        => await publisher.PublishAsync(new UserDeletedIntegrationEvent(domainEvent.UserId), cancellationToken);
}
