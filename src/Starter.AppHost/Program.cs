using Starter.AppHost;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RabbitMQServerResource> rabbitMq = Messaging.AddResources(builder);

Network.AddResources(builder, rabbitMq);

Sales.AddResources(builder, rabbitMq);

builder
    .Build()
    .Run();