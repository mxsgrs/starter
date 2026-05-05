using Microsoft.Extensions.DependencyInjection;
using Network.Application.Authentication.UseCases;
using Network.Application.Shared.Events;
using Network.Application.Users.Dtos;
using Network.Application.Users.Events.Handlers;
using Network.Application.Users.UseCases;

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
        services.AddScoped<IDeleteUserCommandHandler, DeleteUserCommandHandler>();

        // Query handlers
        services.AddScoped<IGenerateTokenQueryHandler, GenerateTokenQueryHandler>();
        services.AddScoped<IReadUserQueryHandler, ReadUserQueryHandler>();

        // Domain event handlers
        services.AddScoped<IPreSavedDomainEventHandler<UserCreatedDomainEvent>, PreUserCreatedDomainEventHandler>();
        services.AddScoped<IPreSavedDomainEventHandler<UserUpdatedDomainEvent>, PreUserUpdatedDomainEventHandler>();
        services.AddScoped<IPreSavedDomainEventHandler<UserDeletedDomainEvent>, PreUserDeletedDomainEventHandler>();
        services.AddScoped<IPostSavedDomainEventHandler<UserCreatedDomainEvent>, PostUserCreatedDomainEventHandler>();
        services.AddScoped<IPostSavedDomainEventHandler<UserDeletedDomainEvent>, PostUserDeletedDomainEventHandler>();
    }
}
