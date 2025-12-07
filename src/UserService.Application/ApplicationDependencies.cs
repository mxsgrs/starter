using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserService.Application.Commands.UserCommands;
using UserService.Application.Queries.AuthCommands;
using UserService.Application.Queries.UserQueries;

namespace UserService.Application;

public static class ApplicationDependencies
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        Assembly assembly = Assembly.Load("UserService.Application");
        services.AddAutoMapper(configuration => configuration.AddMaps(assembly));

        // Command handlers
        services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();

        // Query handlers
        services.AddScoped<IGenerateTokenQueryHandler, GenerateTokenQueryHandler>();
        services.AddScoped<IReadUserQueryHandler, ReadUserQueryHandler>();
    }
}
