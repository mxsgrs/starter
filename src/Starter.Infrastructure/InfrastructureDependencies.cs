using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starter.Domain.Aggregates.UserAggregate;
using Starter.Infrastructure.Persistance;
using Starter.Infrastructure.Persistance.Repositories;

namespace Starter.Infrastructure;

public static class InfrastructureDependencies
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database context
        string connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new Exception("Connection string for SQL Server is missing");

        services.AddDbContext<StarterDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
    }
}
