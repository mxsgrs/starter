using UserService.Domain;

namespace UserService.Application.Shared.Events;

public static class DomainEventPublisherExtensions
{
    public static async Task DispatchAndClearAsync<T>(this IDomainEventPublisher publisher, T aggregate) where T : AggregateRoot
    {
        foreach (IDomainEvent domainEvent in aggregate.DomainEvents)
            await publisher.PublishAsync(domainEvent);

        aggregate.ClearDomainEvents();
    }
}
