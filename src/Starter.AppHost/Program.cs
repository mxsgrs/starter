IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RabbitMQServerResource> rabbitMq = builder
    .AddRabbitMQ("RabbitMq")
    .WithManagementPlugin();

IResourceBuilder<SqlServerServerResource> sqlServer = builder
    .AddSqlServer("SqlServer");

IResourceBuilder<SqlServerDatabaseResource> userDatabase = sqlServer
    .AddDatabase("NetworkDb");

builder.AddProject<Projects.Network_WebApi>("Network")
    .WithReference(rabbitMq)
    .WithReference(userDatabase)
    .WithUrlForEndpoint("https", url => url.Url += "/swagger");

builder
    .AddProject<Projects.Sales_WebApi>("Sales")
    .WithReference(rabbitMq)
    .WithUrlForEndpoint("https", url => url.Url += "/swagger");

builder
    .Build()
    .Run();
