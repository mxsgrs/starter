using Microsoft.Extensions.DependencyInjection;
using Starter.Application.Dtos;
using Starter.Application.Features.AuthenticationFeatures;
using Starter.Application.Features.UserFeatures;
using System.Reflection;

namespace Starter.Application;

public static class ApplicationDependencies
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(UserMapping));

        // MediatR
        Assembly assembly = Assembly.Load("Starter.Application");
        services.AddMediatR(register => register.RegisterServicesFromAssembly(assembly));

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
    }
}
