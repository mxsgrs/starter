using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.Application.Shared.Events;
using UserService.Domain.Aggregates.UserAggregate;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistance;
using UserService.Infrastructure.Persistance.Repositories;

namespace UserService.Infrastructure;

public static class InfrastructureDependencies
{
    public static void AddInfrastructureServices(this IServiceCollection services, 
        IConfiguration configuration, IHostEnvironment environment)
    {
        AddDbContext(services, configuration, environment);

        AddMassTransit(services, configuration);

        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<UserCreatedDomainEvent>, UserCreatedDomainEventHandler>();
        services.AddScoped<ICheckUserAddressService, CheckUserAddressService>();
        services.AddScoped<IUserRepository, UserRepository>();
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
            string connectionString = configuration.GetConnectionString(environment.IsProduction() ? "SqlServer" : "UserDatabase")
                ?? throw new Exception("Connection string for SQL Server is missing");

            services.AddDbContext<UserDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
            });
        }
    }
}
