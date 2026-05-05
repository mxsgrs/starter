using Starter.AppHost.Resources;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<SqlServerServerResource> sqlServer = SqlServer.AddResources(builder);

IResourceBuilder<RabbitMQServerResource> rabbitMq = Messaging.AddResources(builder);

IResourceBuilder<ProjectResource> network = Network.AddResources(builder, sqlServer, rabbitMq);

IResourceBuilder<ProjectResource> sales = Sales.AddResources(builder, sqlServer, rabbitMq);

Gateway.AddResources(builder, network, sales);

builder
    .Build()
    .Run();