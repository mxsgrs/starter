namespace Starter.AppHost.Resources;

public static class Sales
{
    public static IResourceBuilder<ProjectResource> AddResources(IDistributedApplicationBuilder builder, IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        IResourceBuilder<SqlServerServerResource> salesSqlServer = builder
            .AddSqlServer("SalesSqlServer");

        IResourceBuilder<SqlServerDatabaseResource> salesDatabase = salesSqlServer
            .AddDatabase("SalesDb");

        return builder
            .AddProject<Projects.Sales_WebApi>("Sales")
            .WithReference(rabbitMq)
            .WithReference(salesDatabase)
            .WithUrlForEndpoint("https", url => url.Url += "/swagger");
    }
}
