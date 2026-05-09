using MassTransit;
using Network.Application.Shared.Events;

namespace Network.Infrastructure.Messaging;

public class MassTransitIntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    /// <summary>
    /// Publish an integration event to the message bus via MassTransit
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken) where TEvent : class
        => await publishEndpoint.Publish(integrationEvent, cancellationToken);
}
