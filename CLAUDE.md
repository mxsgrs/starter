# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Starter

ASP.NET Core 10 clean-architecture microservices starter (UserService, AddressService) using .NET Aspire, Entity Framework Core, and SQL Server.

Run via .NET Aspire AppHost (`src/AppHost`); tests use Testcontainers for integration and xUnit for unit testing.

## Commands

```bash
# Build
dotnet build Starter.sln

# Run via Aspire (starts SQL Server, RabbitMQ, and all services in containers)
dotnet run --project src/Starter.AppHost/Starter.AppHost.csproj

# Run all tests
dotnet test Starter.sln

# Run a specific test project
dotnet test tests/UserService.Application.UnitTests/UserService.Application.UnitTests.csproj

# Run a single test by name
dotnet test --filter "FullyQualifiedName~Handle_ShouldCreateUser_WhenValidInput"

# Add an EF Core migration for Network service (run from repo root)
# Use Network.Infrastructure as both --project and --startup-project.
# Do NOT add Microsoft.EntityFrameworkCore.Design to Network.WebApi — that would violate Clean Architecture
# (the Presentation layer must not depend on EF Core tooling).
# Network.Infrastructure contains IDesignTimeDbContextFactory<NetworkDbContext> which provides the design-time context.
ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add <MigrationName> --project src/Network.Infrastructure --startup-project src/Network.Infrastructure --output-dir Persistance/Migrations

# Add an EF Core migration for Sales service (run from repo root)
ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add <MigrationName> --project src/Sales.WebApi --startup-project src/Sales.WebApi
```

## Architecture

### Projects

| Project | Layer | Role |
|---|---|---|
| `UserService.Domain` | Domain | Aggregates, domain events, repository interfaces, value objects |
| `UserService.Application` | Application | CQRS command/query handlers, DTOs, DI registration |
| `UserService.Infrastructure` | Infrastructure | EF Core DbContext, repositories, MassTransit, external service clients |
| `UserService.WebApi` | Presentation | ASP.NET Core controllers, JWT auth, DI composition root |
| `AddressService.WebApi` | Microservice | Address validation HTTP API + MassTransit consumers |
| `Starter.AppHost` | Orchestration | .NET Aspire host — wires SQL Server, RabbitMQ, and services |
| `Starter.ServiceDefaults` | Cross-cutting | Shared Aspire observability/health extensions |

Test projects mirror the layer under test: `UserService.Domain.UnitTests`, `UserService.Application.UnitTests`, `UserService.Infrastructure.UnitTests`, `UserService.WhiteBoxE2eTests`.  
`UserService.ModelBuilders` is a shared test library with fluent builders (e.g., `new UserBuilder().WithFirstName("Jane").Build()`).

### Key patterns

**CQRS** — Commands and queries live in `UserService.Application/Commands/` and `Queries/`. Each operation is a record implementing `ICommand` or `IQuery<TResult>` with a corresponding handler interface and class in the same file. Handlers are registered manually in `ApplicationDependencies.cs`.

**Result pattern** — All operations return `Result` or `Result<T>` (FluentResults). Never throw exceptions for business logic; propagate failures via `Result.Fail(...)`.

**Domain events** — Aggregates inherit `AggregateRoot` which holds a private `List<IDomainEvent>`. Entities call `RaiseDomainEvent(...)` to append events. `NetworkDbContext.SaveChangesAsync` automatically snapshots events from all tracked `AggregateRoot` instances, clears them, then dispatches after a successful save.

**Repository pattern** — Interfaces live in the Domain layer (`IUserRepository`); implementations in Infrastructure. Repositories return `Result<T>` rather than throwing. `SaveChanges()` on the repository delegates to `DbContext.SaveChangesAsync`.

**Mapping** — Mapster is used for DTO mapping. Type configurations are registered in `UserMapping.cs` and applied in `ApplicationDependencies.cs`.

**Messaging** — MassTransit with RabbitMQ handles cross-service integration events. `AddressService` consumes `UserCreatedDomainEvent` via `IConsumer<T>`. MassTransit is configured in `InfrastructureDependencies.cs`.

### DI composition

- `Program.cs` calls `AddApplicationServices()` and `AddInfrastructureServices()`.
- `ApplicationDependencies.cs` registers command handlers, query handlers, and Mapster config.
- `InfrastructureDependencies.cs` registers `NetworkDbContext`, MassTransit, repositories, and external service clients.
- `NetworkDbContext` is only registered in Production; in Development the Aspire host wires the connection string and the context is resolved via the standard `AddDbContext` call from `Program.cs`.

### Testing

- **Unit tests** use in-memory EF Core (`SharedFixture` creates a uniquely named in-memory DB per test class) and Moq for dependencies.
- **Integration tests** (`Network.Infrastructure.IntegrationTests`) use Testcontainers with a real SQL Server container. `SharedFixture` starts one container and creates one database for the entire collection (`[CollectionDefinition("Database")]`), applies EF Core migrations once in `InitializeAsync`, and exposes `CreateDatabaseContext()` to return a fresh `NetworkDbContext` per test. Each test class implements `IDisposable` to delete only the rows it touched via `ExecuteDelete()`.
- **E2E tests** (`WhiteBoxE2eTests`) spin up real SQL Server and RabbitMQ containers via Testcontainers. `StarterWebApplicationFactory` replaces `ICheckUserAddressService` with `AlwaysValidAddressService` and provides `CreateAuthorizedClient()` with a long-lived JWT.
- The E2E factory exposes a `FakeDomainEventPublisher` on `factory.DomainEventPublisher` for asserting on published events.
- **Model builders** — all tests must construct domain objects and DTOs via builders from `Network.ModelBuilders` (e.g. `new UserBuilder().Build()`). Never instantiate aggregates, entities, or DTOs directly in test code. Add a builder to `Network.ModelBuilders` whenever a new buildable type is introduced.

### Configuration

The app reads `appsettings.{ConfigurationName}.json` where `ConfigurationName` comes from an assembly-level attribute (not the standard `ASPNETCORE_ENVIRONMENT`). The `Debug` configuration includes local SQL Server and JWT parameters. In Aspire, connection strings are injected automatically.

## CancellationToken

Do not add `CancellationToken` parameters to methods unless explicitly asked.

## Microservice Independence Rules

- **No project references between microservices.** Each microservice must be a standalone deployable unit. Never add a `ProjectReference` (or equivalent) from one microservice to another.
- **No shared libraries between microservices.** Do not create or reference common/shared class libraries that are consumed by more than one microservice. Duplication is preferred over coupling.
- **Communicate only via contracts.** Microservices interact exclusively through well-defined APIs (HTTP, gRPC, messaging/events). Never via in-process calls or shared code.
- **Own your types.** Each microservice defines its own DTOs, models, and clients — even if they look identical to another service's. Generate them from the contract (OpenAPI/proto/AsyncAPI schema) rather than sharing source.
- **Independent data.** No shared databases, schemas, or direct DB access across microservices.
- **Enforcement:** Reject any PR that introduces a cross-microservice `ProjectReference`, a `Shared`/`Common` project referenced by multiple services, or a NuGet/package dependency built from another microservice's code.

## Code Style

- Always add a XML doc comment on each method:

```csharp
/// <summary>
/// Create a new user in the database
/// </summary>
```

- Model builder chains must be written with each method call on its own line, indented by 4 spaces:

```csharp
// Correct
AuditLog auditLog = new AuditLogBuilder()
    .WithUserId(user.Id)
    .WithEventType(AuditLogEventType.UserCreated)
    .Build();

// Wrong — all on one line
AuditLog auditLog = new AuditLogBuilder().WithUserId(user.Id).WithEventType(AuditLogEventType.UserCreated).Build();
```
