# Network Service — Clean Architecture

This document describes how the Network service is structured around Clean Architecture principles, what each of the four layers is responsible for, and how they interact.

---

## Table of Contents

1. [Overview](#1-overview)
2. [Domain Layer](#2-domain-layer)
3. [Application Layer](#3-application-layer)
4. [Infrastructure Layer](#4-infrastructure-layer)
5. [WebApi Layer](#5-webapi-layer)
6. [Layer Interaction](#6-layer-interaction)
7. [Dependency Rule](#7-dependency-rule)

---

## 1. Overview

The Network service is divided into four projects, each corresponding to a layer of the Clean Architecture:

| Project | Layer | Responsibility |
|---|---|---|
| `Network.Domain` | Domain | Aggregates, value objects, domain events, repository interfaces |
| `Network.Application` | Application | CQRS handlers, DTOs, domain event handlers, integration event contracts |
| `Network.Infrastructure` | Infrastructure | EF Core persistence, MassTransit messaging, domain event dispatch |
| `Network.WebApi` | Presentation | HTTP endpoints, authentication, DI composition root |

Dependencies flow strictly inward: WebApi depends on Application and Infrastructure; Application depends on Domain; Domain depends on nothing.

---

## 2. Domain Layer

**Project:** `Network.Domain`

The Domain layer encodes pure business logic. It has no dependency on EF Core, ASP.NET, or any other framework. Every rule enforced here is a domain rule, not a persistence or transport rule.

### Aggregates

`User` is the only aggregate root. It inherits `AggregateRoot`, which gives it a private event list and the `RaiseDomainEvent` method. All state changes go through factory methods or mutating methods on the aggregate — never direct property assignment from outside.

```csharp
// Three methods, each enforcing invariants before changing state:
User.Create(id, emailAddress, hashedPassword, firstName, ...)  // raises UserCreatedDomainEvent
user.Update(emailAddress, hashedPassword, firstName, ...)      // raises UserUpdatedDomainEvent
user.Delete()                                                  // raises UserDeletedDomainEvent
```

Creating a `User` with an invalid email, a birthday in the future, or a malformed phone number returns a failed `Result<User>` — no exception is thrown, and no event is raised.

### Value Objects

`Address` is a value object that lives inside the `User` aggregate. It inherits `ValueObject<Address>`, which implements structural equality based on `GetEqualityComponents()`. Two `Address` instances with identical fields are equal; identity is irrelevant. Value objects are immutable and carry their own `Create()` factory with validation.

### Domain Events

Three event records represent meaningful state changes on the `User` aggregate:

| Event | Raised when |
|---|---|
| `UserCreatedDomainEvent(UserId)` | A new `User` is successfully created |
| `UserUpdatedDomainEvent(UserId)` | An existing `User` is successfully updated |
| `UserDeletedDomainEvent(UserId)` | A `User` is marked for deletion |

All three implement `IDomainEvent` via the `DomainEvent` abstract record base, which auto-generates an `Id` (Guid) and `CreatedOn` (DateTime) on construction.

Events are held in memory on the aggregate and never leave the Domain layer directly — dispatch is handled by the Infrastructure layer's interceptor.

### Entities

`UserAuditLog` and `SecurityNote` are entities that belong to the `User` aggregate but are not aggregate roots themselves. They are created by domain event handlers rather than by application code directly, keeping their lifecycle coupled to meaningful domain facts.

### Repository Interfaces

Interfaces for all repositories are declared in the Domain layer so that the Application layer can depend on abstractions without touching Infrastructure:

- `IUserRepository` — CRUD operations returning `Result<T>`
- `IAuditLogRepository` — append-only audit log staging
- `ISecurityNoteRepository` — lifecycle management for security notes

The implementations live in Infrastructure; only the contracts live here.

### Authentication Types

`JsonWebTokenParameters` (JWT configuration) and `UserClaims` (claims extracted from a token) are domain types because they express identity concepts that belong to the business domain, not to any specific HTTP or persistence technology.

---

## 3. Application Layer

**Project:** `Network.Application`

The Application layer orchestrates use cases. It knows the Domain layer and declares what it needs from Infrastructure via interfaces, but it never directly calls EF Core, MassTransit, or any other framework.

### CQRS

Every use case is either a command (mutation) or a query (read), expressed as a record and handled by a dedicated handler class:

| Operation | Type | Handler interface |
|---|---|---|
| Create user | `CreateUserCommand` | `ICommandHandlerResultingGuid<CreateUserCommand>` |
| Update user | `UpdateUserCommand` | `ICommandHandler<UpdateUserCommand>` |
| Delete user | `DeleteUserCommand` | `ICommandByIdHandler` |
| Read user | `ReadUserQuery` | `IQueryByIdHandler<Result<UserDto>>` |
| Generate token | `GenerateTokenQuery` | `IQueryHandler<GenerateTokenQuery, Result<LoginResponseDto>>` |

Each handler is in the same file as its command or query record. Handlers are registered manually in `ApplicationDependencies.cs` — no reflection-based auto-discovery.

### DTOs and Mapping

The Application layer defines the shapes of data that cross the boundary between the domain and the outside world:

- `UserDto` — the read representation returned to callers
- `UserWriteDto` — the write representation accepted from callers
- `LoginResponseDto` — the token response after successful authentication

Mapping between domain aggregates and DTOs is handled by Mapster. Type configurations are declared in `UserMapping.cs` and registered in `ApplicationDependencies.cs`.

`UserDtoHelper` encapsulates the non-trivial conversions: it calls `User.Create()` or `user.Update()` with the data from a `UserWriteDto`, so handlers never need to know the signature of the aggregate factory.

### Domain Event Handlers

The Application layer defines what should happen in response to each domain event and provides the handler implementations. There are two handler types, each serving a different consistency boundary:

**Pre-save handlers** (`IPreSavedDomainEventHandler<T>`) run inside the same database transaction as the originating aggregate write. They are used for side-effects that must be atomic with the main write:

| Handler | Triggered by | Effect |
|---|---|---|
| `PreUserCreatedDomainEventHandler` | `UserCreatedDomainEvent` | Inserts `UserAuditLog`; inserts `SecurityNote` if age ≥ 30 |
| `PreUserUpdatedDomainEventHandler` | `UserUpdatedDomainEvent` | Inserts `UserAuditLog`; adds or removes `SecurityNote` based on age |
| `PreUserDeletedDomainEventHandler` | `UserDeletedDomainEvent` | Inserts `UserAuditLog`; removes `SecurityNote` if present |

**Post-save handlers** (`IPostSavedDomainEventHandler<T>`) run after the transaction commits. They are used for cross-service communication, where the action must only happen once the data is durably written:

| Handler | Triggered by | Effect |
|---|---|---|
| `PostUserCreatedDomainEventHandler` | `UserCreatedDomainEvent` | Publishes `UserCreatedIntegrationEvent` |
| `PostUserDeletedDomainEventHandler` | `UserDeletedDomainEvent` | Publishes `UserDeletedIntegrationEvent` |

The dispatch mechanism is implemented in Infrastructure; Application only declares the handler interfaces and the handler classes that implement them.

### Integration Events

`UserCreatedIntegrationEvent(UserId)` and `UserDeletedIntegrationEvent(UserId)` are the contracts published to the message broker. They are defined in the Application layer because they represent application-level facts, not domain-level ones.

The `IIntegrationEventPublisher` interface declares a single `PublishAsync<TEvent>` method. Post-save handlers depend on this interface; the MassTransit implementation lives in Infrastructure.

### IAppContextAccessor

`IAppContextAccessor` provides the current user's claims (`UserClaims`) to any application-layer code that needs to know who is making a request. The implementation (which reads from `HttpContext`) lives in WebApi.

---

## 4. Infrastructure Layer

**Project:** `Network.Infrastructure`

The Infrastructure layer implements everything that touches external systems: the database, the message broker, and the domain event dispatch pipeline. It depends on both the Domain and Application layers but is not depended on by them.

### Persistence

`UserDbContext` is an EF Core `DbContext` with three `DbSet` properties: `Users`, `UserAuditLogs`, and `SecurityNotes`. Entity configurations are defined in separate `IEntityTypeConfiguration<T>` classes:

- `UserConfiguration` — sets the primary key, a unique index on `EmailAddress`, stores `Role` and `Gender` enums as strings, and maps `Address` as an owned entity in a separate `UserAddresses` table.
- `UserAuditLogConfiguration` — sets the primary key, a length constraint on `EventType`, and an index on `UserId`.
- `SecurityNoteConfiguration` — sets the primary key and the foreign key to `User`.

### Repositories

`UserRepository`, `AuditLogRepository`, and `SecurityNoteRepository` implement the interfaces declared in the Domain layer. Repositories return `Result<T>` rather than throwing exceptions, and they delegate all actual persistence to `UserDbContext`. `SaveChangesAsync` is called by the repository — never directly by application handlers — so the domain event interceptor fires at the right time.

### Domain Event Dispatch Pipeline

The domain event pipeline is implemented as an EF Core `SaveChangesInterceptor` (`DomainEventInterceptor`). It hooks two moments in the save lifecycle:

```
UserRepository.SaveChanges()
        │
        ▼
DomainEventInterceptor.SavingChangesAsync()
  ├─ Snapshot events from all tracked AggregateRoot instances
  ├─ Dispatch to pre-save handlers (IPreSavedDomainEventHandler<T>)
  │     → Same DB transaction; audit logs and security notes staged here
  └─ Clear events from aggregates
        │
        ▼
EF Core writes to the database (INSERT / UPDATE / DELETE)
        │
        ▼
DomainEventInterceptor.SavedChangesAsync()
  └─ Dispatch to post-save handlers (IPostSavedDomainEventHandler<T>)
        → After commit; publishes integration events to RabbitMQ
```

`DomainEventDispatcher` resolves handlers dynamically: for each snapshotted event it constructs the closed generic `IPreSavedDomainEventHandler<ConcreteEventType>` at runtime using reflection, then calls all registered implementations via `IServiceProvider.GetServices`. This means adding a new handler for an existing event only requires implementing the interface and registering it in DI.

### Integration Event Publishing

`MassTransitIntegrationEventPublisher` implements `IIntegrationEventPublisher` using MassTransit's `IPublishEndpoint`. It is the single point where a message leaves the service boundary toward RabbitMQ.

### DI Registration

`InfrastructureDependencies.cs` registers everything in this layer: `UserDbContext` with the SQL Server provider and the `DomainEventInterceptor`, repositories, `DomainEventDispatcher`, `MassTransitIntegrationEventPublisher`, and MassTransit configured with the RabbitMQ transport.

---

## 5. WebApi Layer

**Project:** `Network.WebApi`

The WebApi layer is the composition root of the service and the entry point for all HTTP traffic. It depends on both the Application and Infrastructure layers so it can wire the DI container, but it contains no business logic of its own.

### Controllers

All controllers inherit `NetworkControllerBase`, which sets the route prefix to `api/network/[controller]` and applies `[Authorize]` by default. The base class also exposes two helper methods that translate `Result<T>` and `Result` into HTTP responses:

- `CorrespondingStatus<T>(Result<T>)` → `200 OK` with the value, or `400 Bad Request` with the error message
- `CorrespondingStatus(Result)` → `204 No Content`, or `400 Bad Request` with the error message

`UserController` exposes four endpoints:

| Method | Route | Auth | Description |
|---|---|---|---|
| `POST` | `/api/network/user` | Anonymous | Create a new user |
| `GET` | `/api/network/user/{id}` | Required | Read a user by id |
| `PUT` | `/api/network/user/{id}` | Required | Update a user |
| `DELETE` | `/api/network/user/{id}` | Required | Delete a user |

`AuthenticationController` exposes one endpoint at `/api/network/authentication/token` (anonymous) that accepts hashed credentials and returns a JWT.

### Utilities

`AppContextAccessor` implements `IAppContextAccessor` by reading the `sub` claim from `HttpContext.User`. It is registered as scoped so that each request gets its own instance tied to the current `HttpContext`.

`ToKebabParameterTransformer` converts route template tokens to kebab-case so that, for example, a controller named `UserController` routes to `user` rather than `User`.

### Program.cs

`Program.cs` is the composition root. It calls `AddApplicationServices()` and `AddInfrastructureServices()` to register the inner layers, then configures the WebApi-specific concerns:

- **JWT bearer authentication** using HS512, validating issuer, audience, and signature against the values in `JsonWebTokenParameters`
- **OpenAPI/Swagger** with a Bearer security scheme pre-configured
- **Routing** with the kebab-case transformer applied globally

---

## 6. Layer Interaction

The following shows the full path of a `POST /api/network/user` request through all four layers:

```
POST /api/network/user { UserWriteDto }
        │
        ▼ Network.WebApi
UserController.CreateUser()
  └─ Dispatches CreateUserCommand to ICreateUserCommandHandler
        │
        ▼ Network.Application
CreateUserCommandHandler
  ├─ UserDtoHelper.ToUser()
  │     └─ User.Create() — validates, raises UserCreatedDomainEvent
  └─ userRepository.AddAsync(user) → userRepository.SaveChanges()
        │
        ▼ Network.Infrastructure
UserRepository.SaveChangesAsync()
  └─ Triggers DomainEventInterceptor
        │
        ├─ SavingChangesAsync (pre-save, inside transaction)
        │    └─ PreUserCreatedDomainEventHandler
        │         ├─ INSERT UserAuditLog
        │         └─ INSERT SecurityNote (if age ≥ 30)
        │
        ├─ EF Core commits (User + AuditLog + SecurityNote in one transaction)
        │
        └─ SavedChangesAsync (post-save, after commit)
             └─ PostUserCreatedDomainEventHandler
                  └─ MassTransitIntegrationEventPublisher
                       └─ RabbitMQ ← UserCreatedIntegrationEvent
        │
        ▼ Network.WebApi
UserController returns 200 OK { userId }
```

---

## 7. Dependency Rule

The dependency rule of Clean Architecture states that source code dependencies must point inward only. In the Network service this means:

```
Network.WebApi
  → Network.Application
  → Network.Domain

Network.Infrastructure
  → Network.Application
  → Network.Domain
```

- **Domain** knows nothing about Application, Infrastructure, or WebApi.
- **Application** knows Domain, and declares interfaces for what it needs from Infrastructure (`IUserRepository`, `IIntegrationEventPublisher`, `IAppContextAccessor`). It never references Infrastructure types directly.
- **Infrastructure** implements the interfaces declared in Application and Domain. It is the only layer allowed to reference EF Core, MassTransit, and SQL Server.
- **WebApi** wires the DI container by referencing both Application and Infrastructure, so that the concrete implementations registered in Infrastructure satisfy the interfaces expected by Application. It also owns the HTTP-specific configuration (authentication, routing, OpenAPI).

This structure means the Domain and Application layers can be unit-tested without any running database, message broker, or web server — only in-memory fakes and mocks are needed.
