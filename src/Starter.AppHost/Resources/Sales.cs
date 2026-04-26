namespace Starter.AppHost.Resources;

public static class Sales
{
    public static IResourceBuilder<ProjectResource> AddResources(IDistributedApplicationBuilder builder, IResourceBuilder<SqlServerServerResource> sqlServer, IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {

        IResourceBuilder<SqlServerDatabaseResource> salesDatabase = sqlServer
            .AddDatabase("SalesDb");

        return builder
            .AddProject<Projects.Sales_WebApi>("Sales")
            .WithReference(rabbitMq)
            .WithReference(salesDatabase)
            .WaitFor(salesDatabase)
            .WithUrlForEndpoint("https", url => url.Url += "/api/sales/swagger");
    }
}
