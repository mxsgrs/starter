# Starter

ASP.NET Core 10 clean-architecture microservices starter (UserService, AddressService) using .NET Aspire, Entity Framework Core, and SQL Server.

Run via .NET Aspire AppHost (`src/AppHost`); tests use Testcontainers for integration and xUnit for unit testing.

## Microservice Independence Rules
 
- **No project references between microservices.** Each microservice must be a standalone deployable unit. Never add a `ProjectReference` (or equivalent) from one microservice to another.
- **No shared libraries between microservices.** Do not create or reference common/shared class libraries that are consumed by more than one microservice. Duplication is preferred over coupling.
- **Communicate only via contracts.** Microservices interact exclusively through well-defined APIs (HTTP, gRPC, messaging/events). Never via in-process calls or shared code.
- **Own your types.** Each microservice defines its own DTOs, models, and clients — even if they look identical to another service's. Generate them from the contract (OpenAPI/proto/AsyncAPI schema) rather than sharing source.
- **Independent data.** No shared databases, schemas, or direct DB access across microservices.
- **Enforcement:** Reject any PR that introduces a cross-microservice `ProjectReference`, a `Shared`/`Common` project referenced by multiple services, or a NuGet/package dependency built from another microservice's code.

## Code Style

- Always add a comment on each method

```
/// <summary>
/// Create a new user in the database
/// </summary>
```
