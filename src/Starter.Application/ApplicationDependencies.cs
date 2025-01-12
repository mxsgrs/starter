using Microsoft.Extensions.DependencyInjection;
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
    }
}
