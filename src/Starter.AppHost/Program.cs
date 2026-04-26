using Starter.AppHost.Resources;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> sqlPassword = builder.AddParameter("SqlPassword", secret: true);

IResourceBuilder<SqlServerServerResource> sqlServer = builder
    .AddSqlServer("SqlServer", password: sqlPassword, port: 14330);

IResourceBuilder<RabbitMQServerResource> rabbitMq = Messaging.AddResources(builder);

IResourceBuilder<ProjectResource> network = Network.AddResources(builder, sqlServer, rabbitMq);

IResourceBuilder<ProjectResource> sales = Sales.AddResources(builder, sqlServer, rabbitMq);

Gateway.AddResources(builder, network, sales);

builder
    .Build()
    .Run();