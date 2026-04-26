namespace Starter.AppHost;

public static class Sales
{
    public static void AddResources(IDistributedApplicationBuilder builder, IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        IResourceBuilder<SqlServerServerResource> salesSqlServer = builder
            .AddSqlServer("SalesSqlServer");

        IResourceBuilder<SqlServerDatabaseResource> salesDatabase = salesSqlServer
            .AddDatabase("SalesDb");

        builder
            .AddProject<Projects.Sales_WebApi>("Sales")
            .WithReference(rabbitMq)
            .WithReference(salesDatabase)
            .WithUrlForEndpoint("https", url => url.Url += "/swagger");
    }
}
