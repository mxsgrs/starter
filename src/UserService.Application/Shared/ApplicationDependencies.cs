using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Commands.UserCommands;
using UserService.Application.Dtos.UserDtos;
using UserService.Application.Queries.AuthQueries;
using UserService.Application.Queries.UserQueries;

namespace UserService.Application.Shared;

public static class ApplicationDependencies
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Mapster
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        UserMapping.Register(config);

        // Command handlers
        services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();

        // Query handlers
        services.AddScoped<IGenerateTokenQueryHandler, GenerateTokenQueryHandler>();
        services.AddScoped<IReadUserQueryHandler, ReadUserQueryHandler>();
    }
}
