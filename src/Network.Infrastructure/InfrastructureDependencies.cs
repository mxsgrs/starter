using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Network.Application.Shared.Events;
using Network.Application.Shared.Interfaces;
using Network.Domain.Aggregates.FinancialProfileAggregate;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Messaging;
using Network.Infrastructure.Persistance;
using Network.Infrastructure.Persistance.Repositories;

namespace Network.Infrastructure;

public static class InfrastructureDependencies
{
    public static void AddInfrastructureServices(this IServiceCollection services, 
        IConfiguration configuration, IHostEnvironment environment)
    {
        AddDbContext(services, configuration, environment);

        AddMassTransit(services, configuration);

        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IFinancialProfileRepository, FinancialProfileRepository>();
        services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();
    }

    /// <summary>
    /// Configure MassTransit DI
    /// </summary>
    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
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
    }

    /// <summary>
    /// Configure DbContext DI
    /// </summary>
    private static void AddDbContext(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        string? aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if ((environment.IsDevelopment() && aspNetCoreEnvironment is not null) || environment.IsProduction())
        {
            // docker run -d -e ACCEPT_EULA=Y -e SA_PASSWORD=B1q22MPXUgosXiqZ -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
            string connectionString = configuration.GetConnectionString(environment.IsProduction() ? "SqlServer" : "NetworkDb")
                ?? throw new Exception("Connection string for SQL Server is missing");

            services.AddDbContext<UserDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
            });
        }
    }
}
