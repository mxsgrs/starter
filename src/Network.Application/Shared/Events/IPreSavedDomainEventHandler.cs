using Network.Domain.Shared.Abstractions;

namespace Network.Application.Shared.Events;

public interface IPreSavedDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handle a domain event; may perform DB writes that will be committed by the caller
    /// </summary>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
