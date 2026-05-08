# Network.Domain

## Regions in Repository Interfaces

Repository interfaces that cover multiple entity concerns must use `#region` blocks to group methods by entity. Each region is named after the entity it covers (e.g. `#region User`, `#region Security Note`).

## Aggregate Folder Naming

Aggregate folders must end with `Aggregate` (e.g. `UserAggregate`, `AuditLogAggregate`). This prevents name conflicts between the folder and the class it contains.

## CancellationToken

Do not add `CancellationToken` parameters to any method unless explicitly asked.

## Aggregate Roots

Aggregates must have no public constructors and no public setters. All state changes go through domain methods (`Create`, `Update`, etc.) that return `Result` or `Result<T>`. Constructors are either `private` (for domain instantiation) or `private` with an EF Core comment (for ORM hydration).

```csharp
// Correct
public static Result<User> Create(...) { ... }
public Result Update(...) { ... }
private User() { } // EF Core

// Wrong — leaks construction and mutation outside the domain
public User(...) { }
public string FirstName { get; set; }

// Properties use private set
public string FirstName { get; private set; } = "";
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
