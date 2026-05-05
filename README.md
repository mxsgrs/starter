# Microservices starter pack

## Dependencies

Before anything, please install the following if not already present:

- Download and install **.NET 10** [here](https://dotnet.microsoft.com/fr-fr/download/dotnet/10.0)
- Download and install **Docker Desktop** [here](https://docs.docker.com/desktop/install/windows-install/)

## Start the project

**.NET Aspire** is an opinionated stack for building observable, production-ready distributed applications. It orchestrates services, databases, and messaging infrastructure locally using containers, and provides a dashboard for logs, traces, and metrics out of the box.

To run this solution, set `Starter.AppHost` as the startup project in your IDE, then click Run. Alternatively, use the CLI:

```bash
# Restore dependencies
dotnet restore Starter.sln

# Run via Aspire (starts SQL Server, RabbitMQ, and all services)
dotnet run --project src/Starter.AppHost/Starter.AppHost.csproj
```

The Aspire dashboard will open automatically and display all running services with their logs and traces.

## Architecture decisions

### Microservices size

Microservices should be deliberately larger in scope rather than highly granular.
This approach reduces excessive cross-service communication, which can otherwise introduce significant latency and performance bottlenecks.
By consolidating related functionality into fewer, more cohesive services, we improve system responsiveness and stability.
It also simplifies the development workflow, as engineers spend less time managing inter-service dependencies and coordination.
Overall, this trade-off favors practical efficiency and developer productivity over strict adherence to fine-grained microservice principles.

### Database per service

Each microservice owns its dedicated SQL Server database, and no service is allowed to query another service's data store directly.
This pattern enforces a hard boundary between domains, ensuring that schema changes in one service cannot break another.
It also enables each service to choose the data model that best fits its needs without compromise.
Consistency across services is achieved through integration events rather than shared transactions.
This comes at the cost of eventual consistency, which is an accepted trade-off in a distributed architecture.

### Async messaging

Cross-service communication relies exclusively on RabbitMQ via MassTransit, using a publish/subscribe model.
Services emit integration events when something meaningful happens (e.g. `UserCreatedDomainEvent`) and consumers react independently.
This removes runtime coupling — a consuming service being down does not affect the publisher, and messages are retried automatically.
It also makes it straightforward to add new consumers without modifying the producer.
The trade-off is increased operational complexity: a message broker must be running and monitored at all times.

### API gateway with path-based routing

A single entry point receives all incoming HTTP traffic and routes requests to the appropriate microservice based on URL path prefixes.
Clients interact with one address regardless of how many services exist behind it, simplifying frontend integration.
Path-based routing (e.g. `/users/*` → UserService, `/addresses/*` → AddressService) makes the routing rules explicit and easy to maintain.
The gateway is also the right place to enforce cross-cutting concerns such as authentication, rate limiting, and TLS termination.
Internal service URLs remain hidden from clients, allowing the topology to evolve without breaking consumers.
