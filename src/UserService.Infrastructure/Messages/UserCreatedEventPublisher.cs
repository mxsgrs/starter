using MassTransit;
using UserService.Domain;
using UserService.Domain.Aggregates.UserAggregate;

namespace UserService.Infrastructure.Messages;

public class UserCreatedEventPublisher(IPublishEndpoint publishEndpoint) : IDomainEventPublisher<UserCreatedEvent>
{
    public async Task PublishAsync(UserCreatedEvent domainEvent)
    {
        await publishEndpoint.Publish(domainEvent);
    }
}
