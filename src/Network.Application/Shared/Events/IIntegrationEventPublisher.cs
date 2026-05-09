namespace Network.Application.Shared.Events;

public interface IIntegrationEventPublisher
{
    /// <summary>
    /// Publish an integration event to the message bus
    /// </summary>
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken) where TEvent : class;
}
