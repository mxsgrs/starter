using UserService.Domain.Events;

namespace UserService.Application.Shared.Events;

public interface IDomainEventDispatcher
{
    /// <summary>
    /// Resolve and invoke all registered handlers for the given domain event
    /// </summary>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
