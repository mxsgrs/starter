using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UserService.Application.Shared.Events;
using UserService.Domain;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging;

public class DomainEventInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    /// <summary>
    /// Snapshot and dispatch domain events before EF Core writes to the database so that
    /// handler side-effects (e.g. audit log entries) are committed in the same transaction
    /// </summary>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            List<IDomainEvent> events = [.. eventData.Context.ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)];

            foreach (IDomainEvent evt in events)
                await dispatcher.DispatchAsync(evt, cancellationToken);

            foreach (EntityEntry<AggregateRoot> entry in eventData.Context.ChangeTracker.Entries<AggregateRoot>())
                entry.Entity.ClearDomainEvents();
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
