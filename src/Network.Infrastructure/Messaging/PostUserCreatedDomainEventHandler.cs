using MassTransit;
using Network.Application.IntegrationEvents;
using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;

namespace Network.Infrastructure.Messaging;

public class PostUserCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : IIntegrationEventPublisher<UserCreatedDomainEvent>
{
    /// <summary>
    /// Publish an integration event to the message bus after the user has been committed to the database
    /// </summary>
    public async Task PublishAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new UserCreatedIntegrationEvent(domainEvent.UserId), cancellationToken);
    }
}
