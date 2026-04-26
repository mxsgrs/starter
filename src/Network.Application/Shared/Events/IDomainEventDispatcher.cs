using Network.Domain.Events;

namespace Network.Application.Shared.Events;

public interface IDomainEventDispatcher
{
    /// <summary>
    /// Resolve and invoke all registered handlers for the given domain event
    /// </summary>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
