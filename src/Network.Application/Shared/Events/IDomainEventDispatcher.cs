using Network.Domain.Events;

namespace Network.Application.Shared.Events;

public interface IDomainEventDispatcher
{
    /// <summary>
    /// Resolve and invoke all pre-save handlers for the given domain event
    /// </summary>
    Task DispatchPreSaveAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Resolve and invoke all post-save integration event publishers for the given domain event
    /// </summary>
    Task DispatchPostSaveAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
