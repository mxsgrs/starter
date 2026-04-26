IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RabbitMQServerResource> rabbitMq = builder
    .AddRabbitMQ("RabbitMq")
    .WithManagementPlugin();

IResourceBuilder<SqlServerServerResource> sqlServer = builder
    .AddSqlServer("SqlServer");

IResourceBuilder<SqlServerDatabaseResource> userDatabase = sqlServer
    .AddDatabase("UserDatabase");

builder
    .AddProject<Projects.UserService_WebApi>("UserService")
    .WithReference(rabbitMq)
    .WithReference(userDatabase);

builder
    .AddProject<Projects.Sales_WebApi>("Sales")
    .WithReference(rabbitMq);

builder
    .Build()
    .Run();
