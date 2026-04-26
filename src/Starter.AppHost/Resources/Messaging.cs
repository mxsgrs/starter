namespace Starter.AppHost.Resources;

public static class Messaging
{
    public static IResourceBuilder<RabbitMQServerResource> AddResources(IDistributedApplicationBuilder builder)
    {
        return builder
            .AddRabbitMQ("RabbitMq")
            .WithManagementPlugin();
    }
}
