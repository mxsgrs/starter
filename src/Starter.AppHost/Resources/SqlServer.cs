namespace Starter.AppHost.Resources;

public static class SqlServer
{
    /// <summary>
    /// Adds the SQL Server resource with a secret password parameter.
    /// </summary>
    public static IResourceBuilder<SqlServerServerResource> AddResources(IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<ParameterResource> sqlPassword = builder.AddParameter("SqlPassword", secret: true);

        return builder
            .AddSqlServer("SqlServer", password: sqlPassword, port: 14330);
    }
}
