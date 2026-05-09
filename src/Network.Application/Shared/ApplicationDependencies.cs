using Microsoft.Extensions.DependencyInjection;
using Network.Application.Authentication.UseCases;
using Network.Application.FinancialProfiles.Dtos;
using Network.Application.FinancialProfiles.UseCases;
using Network.Application.Shared.Events;
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
        FinancialProfileMapping.Register(config);

        // Command handlers
        services.AddScoped<ICreateUserCommandHandler, CreateUserCommandHandler>();
        services.AddScoped<IUpdateUserCommandHandler, UpdateUserCommandHandler>();
        services.AddScoped<IDeleteUserCommandHandler, DeleteUserCommandHandler>();

        // Query handlers
        services.AddScoped<IGenerateTokenQueryHandler, GenerateTokenQueryHandler>();
        services.AddScoped<IReadUserQueryHandler, ReadUserQueryHandler>();

        // FinancialProfile command and query handlers
        services.AddScoped<ICreateAssetCommandHandler, CreateAssetCommandHandler>();
        services.AddScoped<IUpdateAssetCommandHandler, UpdateAssetCommandHandler>();
        services.AddScoped<IDeleteAssetCommandHandler, DeleteAssetCommandHandler>();
        services.AddScoped<IReadFinancialProfileQueryHandler, ReadFinancialProfileQueryHandler>();

        // Domain event handlers
        services.AddScoped<IPreSavedDomainEventHandler<UserCreatedDomainEvent>, PreUserCreatedDomainEventHandler>();
        services.AddScoped<IPreSavedDomainEventHandler<UserUpdatedDomainEvent>, PreUserUpdatedDomainEventHandler>();
        services.AddScoped<IPreSavedDomainEventHandler<UserDeletedDomainEvent>, PreUserDeletedDomainEventHandler>();
        services.AddScoped<IPostSavedDomainEventHandler<UserCreatedDomainEvent>, PostUserCreatedDomainEventHandler>();
        services.AddScoped<IPostSavedDomainEventHandler<UserDeletedDomainEvent>, PostUserDeletedDomainEventHandler>();
    }
}
