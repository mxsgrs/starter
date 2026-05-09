using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PostUserCreatedDomainEventHandler(IIntegrationEventPublisher publisher)
    : IPostSavedDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Publish an integration event to the message bus after the user has been committed to the database
    /// </summary>
    public async Task PublishAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        => await publisher.PublishAsync(new UserCreatedIntegrationEvent(domainEvent.UserId), cancellationToken);
}
