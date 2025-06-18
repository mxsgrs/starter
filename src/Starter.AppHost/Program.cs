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
    .AddProject<Projects.AddressService_WebApi>("AddressService")
    .WithReference(rabbitMq);

builder
    .Build()
    .Run();
