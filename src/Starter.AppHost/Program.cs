IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RabbitMQServerResource> messaging = builder
    .AddRabbitMQ("RabbitMq");

IResourceBuilder<SqlServerDatabaseResource> startersqldb = builder
    .AddSqlServer("SqlServer")
    .AddDatabase("UserDatabase");

builder
    .AddProject<Projects.Starter_WebApi>("UserService")
    .WithReference(messaging)
    .WithReference(startersqldb);

builder
    .AddProject<Projects.Starter_MockWebApi>("AddressService")
    .WithReference(messaging);

builder
    .Build()
    .Run();
