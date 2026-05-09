# Network.Domain

## Regions in Repository Interfaces

Repository interfaces that cover multiple entity concerns must use `#region` blocks to group methods by entity. Each region is named after the entity it covers (e.g. `#region User`).

## Aggregate Folder Naming

Aggregate folders must end with `Aggregate` (e.g. `UserAggregate`, `AuditLogAggregate`). This prevents name conflicts between the folder and the class it contains.

## CancellationToken

Do not add `CancellationToken` parameters to any method unless explicitly asked.

## Identity Generation

`Create` factory methods must never accept a primary key (`id`) as a parameter. The id must be generated inside the method itself using `Guid.NewGuid()`. The caller is not allowed to supply or influence the identity of the new aggregate or entity.

```csharp
// Correct
public static Result<User> Create(string emailAddress, ...)
{
    User user = new() { Id = Guid.NewGuid(), ... };
    ...
}

// Wrong — caller controls the id
public static Result<User> Create(Guid id, string emailAddress, ...)
```

`Update` methods are subject to the same rule: they must never accept a primary key as a parameter. An aggregate's identity is immutable — once assigned in `Create`, it must never change. Accepting an `id` in `Update` would imply the caller can reassign it, which is forbidden.

```csharp
// Correct
public Result Update(string emailAddress, ...)
{
    EmailAddress = emailAddress;
    ...
}

// Wrong — id is not a mutable field
public Result Update(Guid id, string emailAddress, ...)
```

## Aggregate Roots

Every aggregate must inherit from `AggregateRoot`. Aggregates must have no public constructors and no public setters. All state changes go through domain methods (`Create`, `Update`, etc.) that return `Result<T>` (or `Result` for void mutations) — even when there is currently no validation logic. Returning `Result` from the start keeps the signature consistent and avoids breaking callers if validation is added later. Constructors are either `private` (for domain instantiation) or `private` with an EF Core comment (for ORM hydration).

```csharp
// Correct
public class Order : AggregateRoot
{
    public static Result<Order> Create(...) => Result.Ok(new Order { ... });
    public Result Cancel() { ... return Result.Ok(); }
    private Order() { } // EF Core
}

// Wrong — missing base class, plain return type, public constructor/setter
public class Order
{
    public static Order Create(...) => new Order { ... };
    public Order(...) { }
    public string Status { get; set; }
}

// Properties use private set
public string Status { get; private set; } = "";
```

## Value Objects

Value objects have no identity and must be immutable. Never add mutation methods (e.g. `UpdateFrom`, `Update`) to a value object. To "update" a value object on an aggregate, replace the reference entirely:

```csharp
// Correct
Address = newAddress;

// Wrong — mutates in place, violates immutability
Address.UpdateFrom(newAddress);
```

Value objects must not raise domain events. Only aggregates have the identity and lifecycle needed to produce meaningful events. If a state change warrants an event, it belongs on the aggregate that owns the value object.

## FinancialProfile Aggregate Rules

- `FinancialProfile` is the aggregate root; `Asset` is an entity owned by it — not a value object. Assets have identity and are independently mutable via `UpdateAsset`.
- `RiskScore` is always recomputed inside the aggregate via the private `RecalculateRiskScore()` method immediately after any asset add, update, or remove. Never set `RiskScore` directly from outside the aggregate.
- `Asset` factory (`Create`) and mutator (`Update`) methods are `internal` — only `FinancialProfile` may create or update `Asset` instances.
- Cross-aggregate side effects (creating a `FinancialProfile` when a `User` is created) are handled via domain event handlers in the application layer, not from within either aggregate.
- Deletion of `FinancialProfile` and its `Asset` children when a `User` is deleted is handled by EF Core cascade delete — no domain event handler or explicit repository call is needed for this.
