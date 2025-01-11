using Microsoft.Extensions.DependencyInjection;
using Starter.Application.Features.AuthenticationFeatures;
using Starter.Application.Features.UserFeatures;

namespace Starter.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(UserMapping));

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
    }
}
