using Network.Domain.Events;

namespace Network.Application.Shared.Events;

public interface IPostSavedDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Publish an integration event to the message bus after the DB transaction has committed
    /// </summary>
    Task PublishAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
