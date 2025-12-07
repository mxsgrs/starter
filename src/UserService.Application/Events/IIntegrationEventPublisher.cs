namespace UserService.Application.Events;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IIntegrationEvent;
}
