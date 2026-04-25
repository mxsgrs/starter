using MassTransit;
using UserService.Application.Shared.Events;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging;

public class DomainEventPublisher(IPublishEndpoint publishEndpoint) : IDomainEventPublisher
{
    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        await publishEndpoint.Publish(domainEvent);
    }
}
