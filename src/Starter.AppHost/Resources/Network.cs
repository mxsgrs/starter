namespace Starter.AppHost.Resources;

public static class Network
{
    public static IResourceBuilder<ProjectResource> AddResources(IDistributedApplicationBuilder builder, IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        IResourceBuilder<SqlServerServerResource> networkSqlSerever = builder
            .AddSqlServer("NetworkSqlServer");

        IResourceBuilder<SqlServerDatabaseResource> userDatabase = networkSqlSerever
            .AddDatabase("NetworkDb");

        return builder.AddProject<Projects.Network_WebApi>("Network")
            .WithReference(rabbitMq)
            .WithReference(userDatabase)
            .WaitFor(userDatabase)
            .WithUrlForEndpoint("https", url => url.Url += "/api/network/swagger");
    }
}
