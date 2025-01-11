using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starter.Domain.Aggregates.UserAggregate;
using Starter.Infrastructure.Persistance;
using Starter.Infrastructure.Persistance.Repositories;

namespace Starter.Infrastructure;

public static class InfrastructureDependencies
{
    public static void AddInfrastructureServices(this IServiceCollection services, 
        IConfiguration configuration, IHostEnvironment environment)
    {
        string? aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Database context
        if (environment.IsProduction())
        {
            string connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new Exception("Connection string for SQL Server is missing");

            services.AddDbContext<StarterDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // Message bus
        services.AddMassTransit(registration =>
        {
            registration.UsingAzureServiceBus((context, configurator) =>
            {
                string connectionString = configuration.GetConnectionString("AzureServiceBus")
                    ?? throw new Exception("Azure Service Bus connection string is missing");

                configurator.Host(connectionString);
                configurator.ConfigureEndpoints(context);
            });
        });

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
