namespace Starter.AppHost;

public static class Network
{
    public static void AddResources(IDistributedApplicationBuilder builder, IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        IResourceBuilder<SqlServerServerResource> networkSqlSerever = builder
            .AddSqlServer("NetworkSqlServer");

        IResourceBuilder<SqlServerDatabaseResource> userDatabase = networkSqlSerever
            .AddDatabase("NetworkDb");

        builder.AddProject<Projects.Network_WebApi>("Network")
            .WithReference(rabbitMq)
            .WithReference(userDatabase)
            .WithUrlForEndpoint("https", url => url.Url += "/swagger");
    }
}
