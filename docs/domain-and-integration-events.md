# Domain Events & Integration Events

## What Are They?

**Domain events** represent something significant that happened *within* the domain — a fact, past-tense and immutable. They are raised by aggregates themselves as part of executing a business operation. They carry the intent of the domain ("a user was created") rather than being an implementation artefact.

**Integration events** carry that same fact *across service boundaries* via the message bus. Where a domain event is an in-process signal, an integration event is the contract published to RabbitMQ so that other microservices (e.g. `Sales.WebApi`) can react independently.

The two types solve different problems: domain events keep side-effects inside a single service transactionally consistent; integration events propagate facts to the outside world asynchronously.

---

## How Domain Events Are Created

Domain events are raised by aggregate methods, not by application code. The aggregate is the only source of truth for what counts as a "meaningful thing that happened."

In `User.cs`, the three domain methods each end with a `RaiseDomainEvent(...)` call:

```csharp
// User.cs:87 — Create
user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

// User.cs:124 — Update
RaiseDomainEvent(new UserUpdatedDomainEvent(Id));

// User.cs:137 — Delete
RaiseDomainEvent(new UserDeletedDomainEvent(Id));
```

Crucially, the event is only raised *after* validation passes (in `Create` and `Update`). An invalid user never emits an event. This means the domain guarantees that any event that exists corresponds to a real, valid state change.

The base class `AggregateRoot` holds the events in a private list and exposes them read-only:

```csharp
// AggregateRoot.cs:12-18
private readonly List<IDomainEvent> _domainEvents = [];
public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
public void ClearDomainEvents() => _domainEvents.Clear();
```

Events accumulate in memory on the aggregate until the repository calls `SaveChangesAsync`, at which point the EF Core interceptor takes over.

---

## How Domain Events Are Dispatched

Dispatch is wired entirely through an EF Core `SaveChangesInterceptor` — `DomainEventInterceptor`. The interceptor hooks two moments in the save lifecycle: just **before** EF writes to the database (`SavingChangesAsync`) and just **after** the transaction commits (`SavedChangesAsync`).

### The full flow

```
Application layer calls repository.SaveChanges()
        │
        ▼
DomainEventInterceptor.SavingChangesAsync()
  ├─ Snapshot events from all tracked AggregateRoot instances
  ├─ Call dispatcher.DispatchPreSaveAsync(evt) for each event
  │     └─ Resolves IPreSavedDomainEventHandler<TEvent> from DI
  │         └─ Calls handler.HandleAsync(evt)   ← same DB transaction
  └─ Clear events from aggregates (prevents double-dispatch)
        │
        ▼
EF Core writes to the database (INSERT / UPDATE / DELETE)
        │
        ▼
DomainEventInterceptor.SavedChangesAsync()
  └─ Call dispatcher.DispatchPostSaveAsync(evt) for each snapshot event
        └─ Resolves IPostSavedDomainEventHandler<TEvent> from DI
            └─ Calls handler.PublishAsync(evt)   ← after commit
```

`DomainEventDispatcher` resolves handlers dynamically using reflection, constructing the closed generic `IPreSavedDomainEventHandler<UserCreatedDomainEvent>` at runtime:

```csharp
// DomainEventDispatcher.cs:28-34
Type handlerType = openGenericType.MakeGenericType(domainEvent.GetType());
MethodInfo handleMethod = handlerType.GetMethod(methodName)!;

foreach (object? handler in serviceProvider.GetServices(handlerType))
    await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
```

Multiple handlers for the same event type are all invoked in sequence. Adding a new handler is just a matter of implementing the interface and registering it in DI — no changes to the dispatcher.

---

## Pre-Save vs Post-Save: Why Both?

The two phases exist because some side-effects *must* be atomic with the main write, and others *must not* happen before the data is committed.

### Pre-save handlers (`IPreSavedDomainEventHandler<T>`)

Pre-save handlers run inside the same database transaction as the originating write. Any entity they create or modify is committed together with the triggering aggregate. If the main save fails, everything rolls back — including the side-effects.

**Use it when:** the side-effect belongs to the same consistency boundary. Examples:
- Audit logs (you never want a user created without a log entry)
- Derived entities computed from the new state (e.g. `SecurityNote` based on user age)

**Example — `PreUserCreatedDomainEventHandler`:**

```csharp
// Runs before the INSERT. Both the audit log and security note
// are committed in the same transaction as the new User row.
public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
{
    UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserCreatedDomainEvent));
    await auditLogRepository.AddAsync(auditLog, cancellationToken);

    Result<User> userResult = await userRepository.ReadTrackedUser(domainEvent.UserId);
    if (userResult.IsSuccess && userResult.Value.Age >= 30)
    {
        SecurityNote note = SecurityNote.Create(domainEvent.UserId, "User age is 30 or above");
        await securityNoteRepository.AddAsync(note, cancellationToken);
    }
}
```

If the user save rolls back (e.g. a unique constraint violation), neither the audit log nor the security note is written. No cleanup code needed.

### Post-save handlers (`IPostSavedDomainEventHandler<T>`)

Post-save handlers run *after* the transaction has committed. By definition they cannot be part of the transaction. Their purpose is to notify the outside world once the fact is durable.

**Use it when:** the action involves crossing a service boundary — publishing to the message bus, calling an external HTTP API, sending an email. You must only do this after the data is committed, otherwise you'd be telling other services about something that might not have been persisted.

**Example — `PostUserCreatedDomainEventHandler`:**

```csharp
// Runs after the INSERT commits. By this point the user
// definitely exists in the DB, so other services can safely react.
public async Task PublishAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    => await publisher.PublishAsync(new UserCreatedIntegrationEvent(domainEvent.UserId), cancellationToken);
```

`Sales.WebApi` will eventually receive `UserCreatedIntegrationEvent` and replicate the user into its own database. If the message bus publish fails here, the user is still created — the integration is eventually consistent by design.

### Summary

| | Pre-save | Post-save |
|---|---|---|
| When | Before EF writes | After transaction commits |
| Transaction | Same as originating write | Outside the transaction |
| Failure behaviour | Rolls back everything | Main write already committed |
| Typical use | Audit logs, derived entities | Integration event publishing, emails, webhooks |

---

## Why Domain Events Are Synchronous (And Why That Matters)

Both pre-save and post-save domain event handlers are invoked **synchronously within the same request** — not via a queue, not on a background thread. This is a deliberate choice with significant benefits.

### What you avoid with synchronous dispatch

**No message ordering problems.** Async event systems require careful thought about ordering guarantees. What if event B is processed before event A? With synchronous dispatch inside a single request, events are always processed in the order they were raised, within the same unit of work.

**No duplicate-processing complexity.** Message buses deliver "at least once". This means consumers must be idempotent. Synchronous handlers run exactly once per save — no idempotency logic needed for in-process side-effects.

**No eventual-consistency bugs in the same service.** If `PreUserCreatedDomainEventHandler` were async and queued, there would be a window between the user insert and the audit log insert where the system is inconsistent. A synchronous pre-save handler closes that window entirely: the audit log is either there with the user, or neither is.

**Debuggability.** A single stack trace covers the command handler, the repository save, the domain event handler, and the audit log insert. With async events you'd need to trace across worker threads and correlate message IDs.

**Transactional integrity.** The most powerful property: pre-save handlers participate in the EF Core transaction. If anything in the handler throws, the entire unit of work (including the aggregate insert) is rolled back. No compensating transactions, no saga patterns, no cleanup jobs — the database is never left in a half-written state.

### The trade-off

Synchronous handlers add latency to the HTTP request. If a handler does expensive I/O (e.g. calling a third-party API), the user waits. The pattern therefore works best for fast, local side-effects. Cross-service communication stays asynchronous (post-save + message bus) because that's where eventual consistency is acceptable and the latency would be unacceptable in a transaction.

The split between pre-save and post-save is exactly this trade-off made explicit: fast, local, must-be-consistent → pre-save synchronous; slow, remote, eventually-consistent → post-save async via message bus.

---

## End-to-End Example: Creating a User

```
POST /users
        │
        ▼
CreateUserCommandHandler
  └─ User.Create(...)          ← validates, raises UserCreatedDomainEvent
        │
        ▼
userRepository.AddAsync(user)  ← schedules INSERT, does not save yet
        │
        ▼
userRepository.SaveChanges()   ← triggers DomainEventInterceptor
        │
  ┌─────┴──────────────────────────────────────────────┐
  │  SavingChangesAsync (pre-save, inside transaction)  │
  │  ├─ snapshot [UserCreatedDomainEvent]                │
  │  └─ PreUserCreatedDomainEventHandler.HandleAsync()  │
  │       ├─ INSERT UserAuditLog                        │
  │       └─ INSERT SecurityNote (if age ≥ 30)          │
  └─────┬──────────────────────────────────────────────┘
        │
        ▼
EF Core commits transaction
  → User row, AuditLog row, SecurityNote row all written atomically
        │
  ┌─────┴──────────────────────────────────────────────┐
  │  SavedChangesAsync (post-save, after commit)        │
  │  └─ PostUserCreatedDomainEventHandler.PublishAsync()│
  │       └─ IIntegrationEventPublisher.PublishAsync()  │
  │             └─ MassTransit → RabbitMQ               │
  └─────┬──────────────────────────────────────────────┘
        │
        ▼
Sales.WebApi — UserCreatedEventConsumer.Consume()
  └─ userRepository.AddAsync(...)   ← replicates user in Sales DB
```

At no point is application code responsible for remembering to dispatch events — raising the event on the aggregate and calling `SaveChanges()` is sufficient. The interceptor handles everything else.
