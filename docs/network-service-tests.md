# Network Service — Tests

This document describes the test strategy for the Network service, the role of each test project, and how the shared infrastructure is organised.

---

## Table of Contents

1. [Overview](#1-overview)
2. [Network.ModelBuilders](#2-networkmodelbuilders)
3. [Network.Domain.UnitTests](#3-networkdomainunittests)
4. [Network.Application.UnitTests](#4-networkapplicationunittests)
5. [Network.Infrastructure.IntegrationTests](#5-networkinfrastructureintegrationtests)
6. [Network.WhiteBoxE2eTests](#6-networkwhiteboxe2etests)

---

## 1. Overview

The Network service has four test projects, each targeting a different layer, plus a shared utility library:

| Project | Kind | Tools | What it tests |
|---|---|---|---|
| `Network.ModelBuilders` | Utility | — | Fluent builders for test data |
| `Network.Domain.UnitTests` | Unit | xUnit | Aggregate validation and domain event raising |
| `Network.Application.UnitTests` | Unit | xUnit, Moq | CQRS handlers with mocked dependencies |
| `Network.Infrastructure.IntegrationTests` | Integration | xUnit, Testcontainers | Repository implementations against a real SQL Server |
| `Network.WhiteBoxE2eTests` | End-to-end | xUnit, Testcontainers, WebApplicationFactory | HTTP endpoints, database state, and domain event side-effects |

The test pyramid is deliberately narrow at the top: expensive E2E tests cover the happy path and critical flows; the bulk of the coverage sits in the cheaper unit and integration layers.

---

## 2. Network.ModelBuilders

`Network.ModelBuilders` is a shared test library, not a test runner. It provides fluent builder classes that any other test project can use to construct valid domain objects and DTOs without repeating field-by-field setup.

Each builder ships with sensible defaults so a test only needs to override the properties it cares about:

```csharp
User user = new UserBuilder()
    .WithEmailAddress("jane@example.com")
    .WithBirthday(new DateOnly(1985, 6, 15))
    .Build();
```

| Builder | Produces | Notes |
|---|---|---|
| `UserBuilder` | `User` aggregate | `Build()` unwraps `Result<User>`; `BuildResult()` returns the raw result; `BuildUpdateResult()` applies an update and clears domain events |
| `AddressBuilder` | `Address` value object | Suitable defaults for all six address fields |
| `UserDtoBuilder` | `UserDto` | Read-side DTO for assertion comparisons |
| `UserWriteDtoBuilder` | `UserWriteDto` | Request-side DTO for POST/PUT payloads |

---

## 3. Network.Domain.UnitTests

### What is tested

Domain unit tests verify the invariants and business rules enforced by the `User` aggregate in complete isolation — no database, no mocks, no DI container.

### Setup

There is no shared fixture. Each `[Fact]` constructs the inputs it needs directly, typically via `UserBuilder`, and calls the aggregate factory or mutating method under test.

### Test cases

**`UserTests`** covers `User.Create()`:

| Test | Asserts |
|---|---|
| Valid inputs | `result.IsSuccess`, all properties set correctly |
| Birthday in the future | `result.IsFailed` |
| Invalid email format | `result.IsFailed` |
| Invalid phone format | `result.IsFailed` |
| First name exceeds 128 characters | `result.IsFailed` |
| Valid inputs | `UserCreatedDomainEvent` is raised with the correct `UserId` |

**`UserUpdateTests`** covers `user.Update()`:

| Test | Asserts |
|---|---|
| Valid inputs | All mutable properties updated; `Id` unchanged |
| Birthday in the future | `result.IsFailed` |
| Invalid email format | `result.IsFailed` |
| Invalid phone format | `result.IsFailed` |
| First name exceeds 128 characters | `result.IsFailed` |
| Valid inputs | `UserUpdatedDomainEvent` is raised with the correct `UserId` |

### Patterns

- The `Result<T>` pattern is used throughout: tests check `result.IsSuccess` or `result.IsFailed` rather than catching exceptions.
- Domain events are asserted via `user.DomainEvents`: `Assert.Single(user.DomainEvents)` confirms exactly one event was raised, and the event's properties are inspected directly.

---

## 4. Network.Application.UnitTests

### What is tested

Application unit tests verify that CQRS handlers orchestrate their dependencies correctly: they call the right repository methods in the right order, propagate failures, and return the expected result. No real database or message broker is involved.

### Setup

**`SharedFixture`** is an `IClassFixture` used by handlers that need configuration or a caller identity:

- `Configuration` — loads `appsettings.json` so handlers like `GenerateTokenQueryHandler` can read JWT parameters.
- `AppContextAccessor` — a mock `IAppContextAccessor` pre-seeded with a `UserClaims` object, providing a caller identity without an HTTP context.

Repository and other infrastructure dependencies are mocked with Moq inside each test class.

### Test cases

**`CreateUserCommandHandlerTests`**

| Test | Asserts |
|---|---|
| Valid input, repository succeeds | `result.IsSuccess`; returned value equals `user.Id`; `AddAsync` called once with correct email |
| Repository returns failure | `result.IsFailed`; failure propagated unchanged |

**`UpdateUserCommandHandlerTests`**

| Test | Asserts |
|---|---|
| Valid input, repository succeeds | `result.IsSuccess`; `UpdateAsync` called once |
| User not found | `result.IsFailed`; `UpdateAsync` never called |
| Repository update fails | `result.IsFailed` |

**`DeleteUserCommandHandlerTests`**

| Test | Asserts |
|---|---|
| Valid input, repository succeeds | `result.IsSuccess`; `RemoveAsync` called once |
| User not found | `result.IsFailed`; `RemoveAsync` never called |
| Repository delete fails | `result.IsFailed` |

**`ReadUserQueryHandlerTests`**

| Test | Asserts |
|---|---|
| User exists | `result.IsSuccess`; `UserDto` properties match the domain object (Mapster mapping verified) |
| User not found | `result.IsFailed` |

Mapster type configurations are registered in the constructor (`UserMapping.Register(TypeAdapterConfig.GlobalSettings)`) so mapping logic is exercised in these tests.

**`GenerateTokenQueryHandlerTests`** (uses `SharedFixture`)

| Test | Asserts |
|---|---|
| Invalid credentials | `result.IsFailed` |
| Valid credentials | `result.IsSuccess`; `AccessToken` is non-null and non-empty |

### Patterns

- Moq `Setup` / `Returns` for happy paths; `ReturnsAsync(Result.Fail(...))` for failure paths.
- `Verify(Times.Once)` confirms that side-effecting calls (writes, deletes) are made exactly once.
- `Verify(Times.Never)` confirms that downstream calls are skipped when a guard check fails.

---

## 5. Network.Infrastructure.IntegrationTests

### What is tested

Integration tests verify repository implementations against a real SQL Server instance: that SQL queries are correct, that EF Core configurations map entities as expected, and that `Result<T>` error messages are accurate.

### Setup

**`SharedFixture`** starts a single Testcontainers SQL Server 2022 container for the entire test collection and applies EF Core migrations once in `InitializeAsync`. It exposes `CreateDatabaseContext()`, which returns a fresh `UserDbContext` per call to avoid EF Core identity-map interference between tests.

All test classes carry `[Collection("Database")]` so xUnit assigns them to the same collection fixture and shares the container:

```csharp
[CollectionDefinition("Database")]
public sealed class DatabaseCollection : ICollectionFixture<SharedFixture> { }

[Collection("Database")]
public class AddAsyncTests(SharedFixture fixture) : IDisposable
{
    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.Users.ExecuteDelete();  // bulk-delete only the rows this test touched
    }
}
```

Each test class implements `IDisposable` and bulk-deletes only the rows it inserted, leaving the schema intact for other tests.

### Test cases

**`UserRepository`**

| Class | Tests |
|---|---|
| `AddAsyncTests` | Persists a new user and finds it in the database; returns failure when email is already taken |
| `FindByIdAsyncTests` | Returns the user when the ID exists; returns failure with message `"User not found"` when it does not |
| `FindByCredentialsAsyncTests` | Returns the user when email and hashed password match; returns failure otherwise |
| `RemoveAsyncTests` | Deletes the row and verifies absence; returns failure when the ID does not exist |

**`AuditLogRepository`**

| Class | Tests |
|---|---|
| `AddAsyncTests` | Stages an audit log entry, saves, and verifies the row is in the database with the correct `UserId` and `EventType` |

### Patterns

- One test class per repository method keeps each file small and focused.
- Assertions read directly from a second `UserDbContext` instance rather than relying on the same context used to write, avoiding EF Core cache false positives.
- `ExecuteDelete()` (EF Core bulk delete) is used in `Dispose` to avoid slow row-by-row cleanup.

---

## 6. Network.WhiteBoxE2eTests

### What is tested

E2E tests exercise the full HTTP stack of the Network service: a real ASP.NET Core application is started in-process, backed by a real SQL Server and a real RabbitMQ instance, both managed by Testcontainers. Tests send HTTP requests and assert both on the HTTP response and on the resulting database state, including domain event side-effects such as audit logs.

### Setup

**`StarterWebApplicationFactory`** extends `WebApplicationFactory<Program>` and implements `IAsyncLifetime`:

- Starts a SQL Server 2022 container and a RabbitMQ container in parallel in `InitializeAsync`.
- Replaces the production `UserDbContext` registration with one that points at the test SQL Server container.
- Overrides the `ConnectionStrings:RabbitMq` configuration key to point at the test RabbitMQ container.
- Runs EF Core migrations once via `MigrateDbContext()` before any test runs.
- Exposes `CreateAuthorizedClient()`, which returns an `HttpClient` with a pre-configured `Authorization: Bearer <token>` header so tests do not need to authenticate manually.

### Test cases

**`UserControllerTests`**

| Test | HTTP call | Asserts |
|---|---|---|
| Create user | `POST /api/network/user` | `200 OK`; response body is a valid Guid; `UserAuditLog` row with `EventType = "UserCreatedDomainEvent"` exists in the database |
| Read user | `GET /api/network/user/{id}` | `200 OK`; all `UserDto` properties (including nested `Address`) match what was inserted |
| Update user | `PUT /api/network/user/{id}` | `204 No Content` |
| Delete user | `DELETE /api/network/user/{id}` | `204 No Content`; user row absent from database; `UserAuditLog` row with `EventType = "UserDeletedDomainEvent"` exists |

**`AuthenticationControllerTests`**

| Test | HTTP call | Asserts |
|---|---|---|
| Valid credentials | `POST /api/network/authentication/token` | `200 OK`; response JSON contains a non-empty `accessToken` field |
| Invalid credentials | `POST /api/network/authentication/token` | `400 Bad Request` |

### Patterns

- Database state is asserted after every mutating request by querying through a fresh `UserDbContext`, not just by reading the HTTP response. This catches cases where the handler returns a success code but fails to write.
- Audit log assertions verify that the domain event pipeline fired correctly end-to-end, not just that the aggregate raised an event in memory.
- Each test class implements `IAsyncLifetime` and calls `ExecuteDeleteAsync()` in `DisposeAsync` to clean up rows between test runs without restarting the containers.
