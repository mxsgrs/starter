namespace UserService.Application.Shared.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent);
}
