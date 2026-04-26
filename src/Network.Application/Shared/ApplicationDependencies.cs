using Microsoft.Extensions.DependencyInjection;
using Network.Application.Commands.UserCommands;
using Network.Application.Dtos.UserDtos;
using Network.Application.Queries.AuthQueries;
using Network.Application.Queries.UserQueries;

namespace Network.Application.Shared;

public static class ApplicationDependencies
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Mapster
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        UserMapping.Register(config);

        // Command handlers
        services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();
        services.AddScoped<IUpdateUserCommandHandler, UpdateUserCommandHandler>();

        // Query handlers
        services.AddScoped<IGenerateTokenQueryHandler, GenerateTokenQueryHandler>();
        services.AddScoped<IReadUserQueryHandler, ReadUserQueryHandler>();
    }
}
