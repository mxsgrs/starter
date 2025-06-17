using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starter.Domain.Aggregates.UserAggregate;
using Starter.Infrastructure.Messages;
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
            registration.UsingRabbitMq((context, rabbitMqConfiguration) =>
            {
                string connectionString = configuration.GetConnectionString("RabbitMq")
                    ?? throw new Exception("Connection string for RabbitMQ is missing");

                rabbitMqConfiguration.Host(new Uri(connectionString));

                rabbitMqConfiguration.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<ICheckUserAddressService, CheckUserAddressService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
