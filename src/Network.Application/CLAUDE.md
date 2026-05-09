# Network.Application

## Domain Event Handlers for Cross-Aggregate Side Effects

When a domain event in one aggregate must trigger state changes in a different aggregate, use `IPreSavedDomainEventHandler<TEvent>`. These handlers run inside the EF Core `SaveChangesInterceptor` before the database write, so they must only call repository methods that **stage** changes (i.e., do not call `SaveChangesAsync` themselves). The interceptor's own `SaveChangesAsync` then commits all staged changes atomically.

**Default rule:** Each domain event has at most one pre-saved handler and one post-saved handler. All business logic for a given phase must be consolidated inside that single handler. Never register multiple `IPreSavedDomainEventHandler<TEvent>` (or `IPostSavedDomainEventHandler<TEvent>`) for the same event type.

```csharp
// Correct — stage only, no SaveChangesAsync
public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
{
    Result<FinancialProfile> profileResult = FinancialProfile.Create(Guid.NewGuid(), domainEvent.UserId);
    if (profileResult.IsSuccess)
        await financialProfileRepository.AddAsync(profileResult.Value); // stages, does not commit
}
```

## FinancialProfile Side Effects

- **User created → FinancialProfile created**: handled inside `PreUserCreatedDomainEventHandler` alongside the audit log write. Both are staged atomically in a single handler.
- **User deleted → FinancialProfile and Assets deleted**: handled by EF Core cascade delete (configured in `FinancialProfileConfiguration`). No domain event handler needed.

## Command and Query Naming

Commands and queries must use CRUD notation. Use `Create`, `Read`, `Update`, `Delete` — never `Add`, `Remove`, `Get`, `Fetch`, or other synonyms.

```csharp
// Correct
CreateAssetCommand
UpdateAssetCommand
DeleteAssetCommand
ReadFinancialProfileQuery

// Wrong
AddAssetCommand
RemoveAssetCommand
GetFinancialProfileQuery
```

## DTO Format

DTOs must be declared as `record` types with `{ get; init; }` properties, not as primary-constructor records. Use `required` for non-nullable string properties. Value types (`Guid`, `decimal`, enums, etc.) do not need `required`.

```csharp
// Correct
public record AssetDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public AssetType AssetType { get; init; }
    public decimal Value { get; init; }
}

// Wrong
public record AssetDto(Guid Id, string Name, AssetType AssetType, decimal Value);
```

## Mapping Profile Location

Mapster mapping configuration classes must live inside the `Dtos/` folder of their feature area, not at the feature root.

```
// Correct
FinancialProfiles/Dtos/FinancialProfileMapping.cs

// Wrong
FinancialProfiles/FinancialProfileMapping.cs
```

## CancellationToken

Do not add `CancellationToken` parameters to `HandleAsync` methods in use cases. Neither the interface signatures nor the handler implementations use it.

```csharp
// Correct
public async Task<Result<Guid>> HandleAsync(CreateUserCommand request)

// Wrong
public async Task<Result<Guid>> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
```
