using Starter.Application.AppContextAccessor;
using Starter.Application.AuthenticationFeatures;
using Starter.Application.UserFeatures;
using Starter.Domain.Aggregates.UserAggregate;
using Starter.Infrastructure.Persistance.Repositories;

namespace Starter.WebApi;

public static class DependencyInjection
{
    public static void AddStarterServices(this IServiceCollection services)
    {
        services.AddScoped<IAppContextAccessor, AppContextAccessor>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
    }
}
