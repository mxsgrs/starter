using MassTransit;
using UserService.Application.Shared.Events;

namespace UserService.Infrastructure.Messaging;

public class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IIntegrationEvent
    {
        await publishEndpoint.Publish(domainEvent);
    }
}
