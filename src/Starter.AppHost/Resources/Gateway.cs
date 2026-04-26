namespace Starter.AppHost.Resources;

public static class Gateway
{
    /// <summary>
    /// Adds the Gateway reverse proxy project and wires it to the Network and Sales services.
    /// </summary>
    public static void AddResources(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> network,
        IResourceBuilder<ProjectResource> sales)
    {
        builder.AddProject<Projects.Gateway_WebApi>("Gateway")
            .WithReference(network)
            .WithReference(sales);
    }
}
