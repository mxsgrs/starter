using MassTransit;
using UserService.Application.Shared.Events;

namespace UserService.Infrastructure.Messages;

public class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IIntegrationEvent
    {
        await publishEndpoint.Publish(domainEvent);
    }
}
