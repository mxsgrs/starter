namespace UserService.Domain;

public interface IDomainEventPublisher<DomainEvent> where DomainEvent : IDomainEvent
{
    Task PublishAsync(DomainEvent domainEvent);
}
