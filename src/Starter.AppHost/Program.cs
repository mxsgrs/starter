using Starter.AppHost.Resources;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RabbitMQServerResource> rabbitMq = Messaging.AddResources(builder);

IResourceBuilder<ProjectResource> network = Network.AddResources(builder, rabbitMq);

IResourceBuilder<ProjectResource> sales = Sales.AddResources(builder, rabbitMq);

Gateway.AddResources(builder, network, sales);

builder
    .Build()
    .Run();