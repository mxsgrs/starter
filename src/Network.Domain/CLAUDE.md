# Network.Domain

## Regions in Repository Interfaces

Repository interfaces that cover multiple entity concerns must use `#region` blocks to group methods by entity. Each region is named after the entity it covers (e.g. `#region User`, `#region Security Note`).

## Aggregate Folder Naming

Aggregate folders must end with `Aggregate` (e.g. `UserAggregate`, `AuditLogAggregate`). This prevents name conflicts between the folder and the class it contains.

## CancellationToken

Do not add `CancellationToken` parameters to any method unless explicitly asked.

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
