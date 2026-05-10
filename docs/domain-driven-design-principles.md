# Domain Driven Design Principles

Domain-Driven Design (DDD) was introduced by Eric Evans in his 2003 book *Domain-Driven Design: Tackling Complexity in the Heart of Software*. Its central idea is to structure code around the core business domain rather than technical concerns. The building blocks Evans defined — value objects, entities, aggregates, and repositories — enforce a consistent model where the domain layer owns all business rules, state changes are explicit, and cross-boundary effects are traceable.

---

## Core Concepts

### Bounded Context

Evans defines a bounded context as an explicit boundary within which a single domain model applies consistently. Inside that boundary every term has one precise meaning; outside it, the same word may refer to something entirely different. The bounded context is the primary unit of strategic DDD — it defines what a team owns end-to-end.

> "A BOUNDED CONTEXT delimits the applicability of a particular model so that team members have a clear and shared understanding of what has to be consistent and how it relates to other contexts."
> — Eric Evans, *Domain-Driven Design* (2003)

**Sizing a bounded context**

A bounded context should map to one team. At a team size of six to seven engineers a context is large enough to own a complete business capability — identity, financial management, sales — while remaining small enough for every member to hold the full model in their head. Splitting contexts below this threshold produces chatty services, distributed-transaction problems, and artificial seams. Merging multiple capabilities into one context above this threshold makes the model unwieldy and turns cross-team coordination into a bottleneck.

**Bounded contexts in this repo**

| Bounded Context | Service | Owns |
|---|---|---|
| Identity & Networking | `Network.WebApi` | Users, financial profiles, audit logs |
| Sales | `Sales.WebApi` | Financial products, contracts |
| Gateway | `Gateway.WebApi` | Routing, authentication edge |

**Cross-context communication**

Bounded contexts may not share code, types, or databases. They communicate exclusively through:

- **Integration events** over RabbitMQ (e.g. `UserCreatedIntegrationEvent` published by `Network`, consumed by `Sales`).
- **HTTP/gRPC APIs** routed through the gateway.

Each context defines its own representation of shared concepts. `Sales` maintains its own local view of a user derived from the integration event — it does not reference `Network`'s `User` type. This is intentional: the two contexts model the same real-world person differently because their responsibilities differ.

---

### Ubiquitous Language

Evans defines ubiquitous language as a shared vocabulary co-developed by domain experts and engineers, used consistently in conversation, documentation, and — critically — in the code itself. When the language in the code drifts from the language the business uses, the model becomes a translation layer rather than a direct expression of the domain, and misunderstandings compound.

> "Use the model as the backbone of a language. Commit the team to exercising that language relentlessly in all communication within the team and in the code. Use the same language in diagrams, writing, and especially speech."
> — Eric Evans, *Domain-Driven Design* (2003)

**In this repo**, class and method names are chosen to mirror the business vocabulary directly:

| Business concept | Code expression |
|---|---|
| A user holds a financial profile | `FinancialProfile` aggregate, created on `UserCreatedDomainEvent` |
| A profile is made up of assets | `Asset` entity inside `FinancialProfile` |
| Each asset carries a risk weight | `RiskFactor` property on `Asset` |
| The portfolio risk is re-evaluated after any asset change | `RecalculateRiskScore()` called in `AddAsset`, `UpdateAsset`, `RemoveAsset` |
| Adding an asset to a profile | `FinancialProfile.AddAsset(...)` |
| Something noteworthy happened to a user | `UserCreatedDomainEvent`, `UserUpdatedDomainEvent`, `UserDeletedDomainEvent` |

The language is also consistent across layers: the command is `CreateUserCommand`, the event is `UserCreatedDomainEvent`, the integration event is `UserCreatedIntegrationEvent` — each name tells you exactly what happened and when in the lifecycle.

---

### Value Object

Evans defines a value object as an object that describes some characteristic or attribute but carries no concept of identity. Two value objects are equal when all their properties are equal. They are always immutable — there are no setters and no mutation methods. To "update" a value object held by an aggregate, replace the reference entirely.

> "An object that represents a descriptive aspect of the domain with no conceptual identity is called a VALUE OBJECT. VALUE OBJECTS are instantiated to represent elements of the design that we care about only for what they are, not who or which they are."
> — Eric Evans, *Domain-Driven Design* (2003)

**Base class:** `ValueObject<T>` (`src/Network.Domain/Shared/Abstractions/ValueObject.cs`)

```csharp
public abstract class ValueObject<T> where T : ValueObject<T>
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
        => obj is T valueObject &&
           GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
}
```

**Example — `Address`** (`src/Network.Domain/Aggregates/UserAggregate/Address.cs`)

`Address` captures a user's postal location. It has no primary key. Two `Address` instances with the same street, city, zip, and country are considered equal.

```csharp
public class Address : ValueObject<Address>
{
    public string AddressLine { get; private set; } = "";
    public string? AddressSupplement { get; private set; }
    public string City { get; private set; } = "";
    public string ZipCode { get; private set; } = "";
    public string? StateProvince { get; private set; }
    public string Country { get; private set; } = "";

    public static Result<Address> Create(string addressLine, string city, string zipCode,
        string country, string? addressSupplement = null, string? stateProvince = null) { ... }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return AddressLine;
        yield return AddressSupplement ?? string.Empty;
        yield return City;
        yield return ZipCode;
        yield return StateProvince ?? string.Empty;
        yield return Country;
    }
}
```

When a user changes their address, the aggregate replaces the reference:

```csharp
// Correct — replace the value object
Address = newAddress;

// Wrong — mutates in place, violates immutability
Address.UpdateFrom(newAddress);
```

---

### Entity

In Evans's terminology, an entity is an object defined by its identity rather than its attributes. An entity has a stable `Id` that persists across state changes. Two entity instances with the same `Id` represent the same domain object regardless of their other properties.

> "Many objects are not fundamentally defined by their attributes, but rather by a thread of continuity and identity."
> — Eric Evans, *Domain-Driven Design* (2003)

**Base class:** `Entity` (`src/Network.Domain/Shared/Abstractions/Entity.cs`)

```csharp
public abstract class Entity
{
    protected static Result Validate(object instance) { ... }
}
```

Entities inside an aggregate must not be creatable or mutable from outside that aggregate. Factory and mutator methods are marked `internal` to enforce this.

**Example — `Asset`** (`src/Network.Domain/Aggregates/FinancialProfileAggregate/Asset.cs`)

`Asset` represents a financial holding (stock, bond, real estate, etc.) owned by a `FinancialProfile`. It has its own `Id` and can be updated independently, but only the `FinancialProfile` aggregate root can create or mutate it.

```csharp
public class Asset : Entity
{
    public Guid Id { get; private set; }
    public Guid FinancialProfileId { get; private set; }
    public string Name { get; private set; } = "";
    public AssetType AssetType { get; private set; }
    public decimal Value { get; private set; }

    /// <summary>Risk weight of this asset. 0 = no risk, 1 = maximum risk.</summary>
    [Range(0.0, 1.0)]
    public decimal RiskFactor { get; private set; }

    // internal — only FinancialProfile can create assets
    internal static Result<Asset> Create(Guid financialProfileId, string name,
        AssetType assetType, decimal value, decimal riskFactor) { ... }

    // internal — only FinancialProfile can update assets
    internal Result Update(string name, AssetType assetType, decimal value, decimal riskFactor) { ... }
}
```

---

### Aggregate

Evans defines an aggregate as a cluster of entities and value objects that form a consistency boundary. All state changes within the boundary must go through the aggregate root — never by mutating child entities directly from outside.

> "An AGGREGATE is a cluster of associated objects that we treat as a unit for the purpose of data changes. Each AGGREGATE has a root and a boundary. The boundary defines what is inside the AGGREGATE. The root is a single, specific ENTITY contained in the AGGREGATE."
> — Eric Evans, *Domain-Driven Design* (2003)

This boundary guarantees that business invariants (e.g. the risk score always reflects the current assets) can be enforced in one place. Evans's rule is that invariants within an aggregate must be kept consistent within a single transaction, while consistency across aggregate boundaries can be eventual.

**Example — `FinancialProfile` with `Asset` children**

`FinancialProfile` is the root; `Asset` is a child entity. The `Asset.Create` and `Asset.Update` methods are `internal`, so external code can never create or modify an asset without going through `FinancialProfile.AddAsset` or `FinancialProfile.UpdateAsset`.

---

### Aggregate Root

Evans designates one entity within each aggregate as the aggregate root — the single public entry point into the cluster. It:

- Exposes the only public factory and mutation methods.
- Enforces all invariants of the aggregate.
- Raises domain events to signal meaningful state transitions.

**Base class:** `AggregateRoot` (`src/Network.Domain/Shared/Abstractions/AggregateRoot.cs`)

```csharp
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

**Aggregate roots in this repo:**

| Aggregate Root | File |
|---|---|
| `User` | `src/Network.Domain/Aggregates/UserAggregate/User.cs` |
| `FinancialProfile` | `src/Network.Domain/Aggregates/FinancialProfileAggregate/FinancialProfile.cs` |
| `AuditLog` | `src/Network.Domain/Aggregates/AuditLogAggregate/AuditLog.cs` |

---

### Factory

Evans defines a factory as any mechanism that encapsulates the construction of a complex object, ensuring it is always created in a valid, consistent state. Factories belong to the domain layer — they enforce invariants at birth so that a partially constructed or invalid aggregate can never exist.

> "When creation of an object, or an entire AGGREGATE, becomes complicated or reveals too much of the internal structure, FACTORIES provide encapsulation."
> — Eric Evans, *Domain-Driven Design* (2003)

Evans distinguishes two common forms: a **factory method** on the aggregate root itself (creation is part of the aggregate's responsibility) and a separate **factory class** (used when the construction logic is complex enough to warrant its own type). In this repo the factory method form is used throughout.

**Pattern in this repo — static `Create` methods**

Every aggregate root and entity exposes a static `Create` factory method. It constructs the object, validates it, and returns `Result<T>` — never a bare constructor, never an exception for a validation failure.

```csharp
// User.cs — aggregate root factory
public static Result<User> Create(string firstName, string lastName, string email, string password)
{
    User user = new()
    {
        Id = Guid.NewGuid(),
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        Password = password
    };

    Result validationResult = Validate(user);
    if (!validationResult.IsSuccess) return Result.Fail<User>(validationResult.Errors);

    user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
    return Result.Ok(user);
}
```

```csharp
// Asset.cs — child entity factory (internal: only the aggregate root may call it)
internal static Result<Asset> Create(Guid financialProfileId, string name,
    AssetType assetType, decimal value, decimal riskFactor)
{
    Asset asset = new()
    {
        Id = Guid.NewGuid(),
        FinancialProfileId = financialProfileId,
        Name = name,
        AssetType = assetType,
        Value = value,
        RiskFactor = riskFactor
    };

    Result validationResult = Validate(asset);
    if (!validationResult.IsSuccess) return Result.Fail<Asset>(validationResult.Errors);

    return Result.Ok(asset);
}
```

Key properties of all factories in this repo:

- **Always valid** — validation runs inside `Create`; a successful `Result<T>` guarantees a fully consistent object.
- **Identity assigned at birth** — `Id = Guid.NewGuid()` is set inside the factory, never by the caller.
- **Domain events raised by the aggregate root** — child entity factories (e.g. `Asset.Create`) return a plain entity; the aggregate root raises any resulting domain event after adding it to the collection.
- **Access controlled** — child entity factories are `internal` so only the owning aggregate root can invoke them.

---

## Rules

### One Repository per Aggregate Root

Evans states that a repository should exist for each aggregate root, and only for aggregate roots — never for child entities. The repository is the only way to load or persist an aggregate. Child entities are always accessed through the aggregate root they belong to.

> "For each type of object that needs global access, create an object that can provide the illusion of an in-memory collection of all objects of that type."
> — Eric Evans, *Domain-Driven Design* (2003)

| Repository Interface | Aggregate Root | File |
|---|---|---|
| `IUserRepository` | `User` | `src/Network.Domain/Aggregates/UserAggregate/IUserRepository.cs` |
| `IFinancialProfileRepository` | `FinancialProfile` | `src/Network.Domain/Aggregates/FinancialProfileAggregate/IFinancialProfileRepository.cs` |
| `IAuditLogRepository` | `AuditLog` | `src/Network.Domain/Aggregates/AuditLogAggregate/IAuditLogRepository.cs` |

`Asset` is an entity, not an aggregate root — there is no `IAssetRepository`. Assets are always loaded and mutated through `FinancialProfile`.

---

## Side Effect Handling

### Intra-Aggregate Side Effects

When a state change inside an aggregate causes another state change **within the same aggregate**, implement it directly in the domain method. No events are needed — the aggregate root coordinates everything internally.

**Example — risk score recalculation in `FinancialProfile`** (`src/Network.Domain/Aggregates/FinancialProfileAggregate/FinancialProfile.cs`)

Every time an asset is added, updated, or removed, the portfolio risk score must be recalculated. This is purely internal to `FinancialProfile`, so it is handled inline by calling the private `RecalculateRiskScore()` method at the end of each mutation:

```csharp
public Result AddAsset(string name, AssetType assetType, decimal value, decimal riskFactor)
{
    Result<Asset> assetResult = Asset.Create(Id, name, assetType, value, riskFactor);
    if (!assetResult.IsSuccess) return Result.Fail(assetResult.Errors);

    _assets.Add(assetResult.Value);
    RecalculateRiskScore(); // side effect, handled in place
    return Result.Ok();
}

public Result UpdateAsset(Guid assetId, string name, AssetType assetType, decimal value, decimal riskFactor)
{
    Asset? asset = _assets.FirstOrDefault(a => a.Id == assetId);
    if (asset is null) return Result.Fail($"Asset {assetId} not found.");

    Result updateResult = asset.Update(name, assetType, value, riskFactor);
    if (!updateResult.IsSuccess) return updateResult;

    RecalculateRiskScore(); // side effect, handled in place
    return Result.Ok();
}

private void RecalculateRiskScore()
{
    decimal totalValue = _assets.Sum(a => a.Value);
    RiskScore = totalValue > 0
        ? _assets.Sum(a => a.Value * a.RiskFactor) / totalValue
        : 0;
}
```

The caller does not need to know that `RiskScore` changed — it is a guaranteed consequence of any asset mutation, enforced by the aggregate itself.

### Cross-Aggregate Side Effects

When a state change in one aggregate must trigger a state change in a **different** aggregate, use domain events. This aligns with Evans's principle that each transaction should modify only one aggregate — cross-aggregate consistency is achieved asynchronously through events. The originating aggregate raises an event; a handler in the application layer reacts to it and performs the secondary operation.

Domain events are dispatched by `DomainEventInterceptor` (`src/Network.Infrastructure/Messaging/DomainEventInterceptor.cs`), an EF Core `SaveChangesInterceptor` that:

1. Snapshots domain events from all tracked aggregate roots before writing to the database.
2. Invokes **pre-save handlers** (`IPreSavedDomainEventHandler<TEvent>`) — these stage additional repository changes that are committed atomically in the same transaction.
3. Writes to the database.
4. Invokes **post-save handlers** (`IPostSavedDomainEventHandler<TEvent>`) — these run after the transaction commits (e.g. publishing to a message bus).

#### Pre-Save Handlers — Atomic Cross-Aggregate Side Effects

Use `IPreSavedDomainEventHandler<TEvent>` when the secondary operation must succeed or fail together with the primary write. Handlers must only call repository methods that **stage** changes without calling `SaveChangesAsync` — the interceptor commits everything in one transaction.

**Example — `AuditLog` and `FinancialProfile` creation on user creation**

When a `User` is created, `UserCreatedDomainEvent` is raised. `PreUserCreatedDomainEventHandler` (`src/Network.Application/Users/Events/Handlers/PreUserCreatedDomainEventHandler.cs`) reacts by staging an `AuditLog` entry and a new `FinancialProfile` — both committed atomically with the user insert:

```csharp
public class PreUserCreatedDomainEventHandler(
    IAuditLogRepository auditLogRepository,
    IFinancialProfileRepository financialProfileRepository)
    : IPreSavedDomainEventHandler<UserCreatedDomainEvent>
{
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, AuditLogEventType.UserCreated);
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value); // stages, does not commit

        Result<FinancialProfile> profileResult = FinancialProfile.Create(domainEvent.UserId);
        if (profileResult.IsSuccess)
            await financialProfileRepository.AddAsync(profileResult.Value); // stages, does not commit
    }
}
```

The same pattern applies when a user is updated or deleted — `PreUserUpdatedDomainEventHandler` and `PreUserDeletedDomainEventHandler` each stage an `AuditLog` entry atomically with the triggering write.

`AuditLog` (`src/Network.Domain/Aggregates/AuditLogAggregate/AuditLog.cs`) is itself an aggregate root, meaning it has its own repository and lifecycle:

```csharp
public class AuditLog : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public AuditLogEventType EventType { get; private set; }
    public DateTime OccurredOn { get; private set; }

    public static Result<AuditLog> Create(Guid userId, AuditLogEventType eventType) =>
        Result.Ok(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventType = eventType,
            OccurredOn = DateTime.UtcNow
        });
}
```

The `User` aggregate has no knowledge of `AuditLog` — the coupling flows only through the domain event and its handler in the application layer.

#### Post-Save Handlers — Out-of-Transaction Side Effects

Use `IPostSavedDomainEventHandler<TEvent>` for side effects that must happen **after** the database transaction commits — typically publishing integration events to a message bus so other services can react.

**Example — publishing an integration event after user creation**

`PostUserCreatedDomainEventHandler` (`src/Network.Application/Users/Events/Handlers/PostUserCreatedDomainEventHandler.cs`) publishes `UserCreatedIntegrationEvent` to RabbitMQ after the user has been persisted:

```csharp
public class PostUserCreatedDomainEventHandler(IIntegrationEventPublisher publisher)
    : IPostSavedDomainEventHandler<UserCreatedDomainEvent>
{
    public async Task PublishAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        => await publisher.PublishAsync(new UserCreatedIntegrationEvent(domainEvent.UserId), cancellationToken);
}
```

Publishing before the commit would risk notifying consumers of a user that was never actually saved.

---

## Quick Reference

| Concept | Definition | Repo Example | Key File |
|---|---|---|---|
| Value Object | Immutable, equality by value, no identity | `Address` | `src/Network.Domain/Aggregates/UserAggregate/Address.cs` |
| Entity | Has identity (`Id`), mutable, equality by Id | `Asset` | `src/Network.Domain/Aggregates/FinancialProfileAggregate/Asset.cs` |
| Aggregate | Consistency boundary, all changes go through root | `FinancialProfile` + `Asset` | `src/Network.Domain/Aggregates/FinancialProfileAggregate/` |
| Aggregate Root | Single entry point, raises domain events | `User`, `FinancialProfile`, `AuditLog` | `src/Network.Domain/Shared/Abstractions/AggregateRoot.cs` |
| Repository | One per aggregate root, never per entity | `IUserRepository`, `IFinancialProfileRepository`, `IAuditLogRepository` | `src/Network.Domain/Aggregates/` |
| Intra-aggregate side effect | Handled directly in domain methods | Risk score recalculation on asset mutation | `FinancialProfile.RecalculateRiskScore()` |
| Cross-aggregate side effect (pre-save) | Domain event + pre-save handler, atomic with primary write | `AuditLog` + `FinancialProfile` created on user creation | `src/Network.Application/Users/Events/Handlers/PreUserCreatedDomainEventHandler.cs` |
| Cross-aggregate side effect (post-save) | Domain event + post-save handler, after DB commit | Integration event published to message bus | `src/Network.Application/Users/Events/Handlers/PostUserCreatedDomainEventHandler.cs` |
| Bounded Context | Explicit boundary where a domain model is consistent; one context per team (~6–7 engineers); communicate via integration events or HTTP only | `Network` (identity), `Sales` (financial products) |
| Ubiquitous Language | Shared business vocabulary used consistently in code, docs, and conversation | `AddAsset`, `RecalculateRiskScore`, `UserCreatedDomainEvent` | Throughout `src/Network.Domain/` |
| Factory | Encapsulates construction of a valid aggregate or entity; always returns `Result<T>` | `User.Create(...)`, `Asset.Create(...)`, `FinancialProfile.Create(...)` | Static `Create` methods on each aggregate/entity |
