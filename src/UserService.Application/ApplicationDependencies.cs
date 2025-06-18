using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserService.Application.Dtos;

namespace UserService.Application;

public static class ApplicationDependencies
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(UserMapping));

        // MediatR
        Assembly assembly = Assembly.Load("UserService.Application");
        services.AddMediatR(register => register.RegisterServicesFromAssembly(assembly));
    }
}
