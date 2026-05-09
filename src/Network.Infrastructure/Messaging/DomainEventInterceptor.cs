using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Network.Application.Shared.Events;
using Network.Domain.Shared.Abstractions;

namespace Network.Infrastructure.Messaging;

public class DomainEventInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    private List<IDomainEvent> _pendingEvents = [];

    /// <summary>
    /// Snapshot and dispatch pre-save handlers before EF Core writes to the database so that
    /// side-effects (e.g. audit log entries) are committed in the same transaction
    /// </summary>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            _pendingEvents = [.. eventData.Context.ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)];

            foreach (IDomainEvent evt in _pendingEvents)
                await dispatcher.DispatchPreSaveAsync(evt, cancellationToken);

            foreach (EntityEntry<AggregateRoot> entry in eventData.Context.ChangeTracker.Entries<AggregateRoot>())
                entry.Entity.ClearDomainEvents();
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Dispatch post-save integration event publishers after the DB transaction has committed
    /// </summary>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        foreach (IDomainEvent evt in _pendingEvents)
            await dispatcher.DispatchPostSaveAsync(evt, cancellationToken);

        _pendingEvents = [];

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
