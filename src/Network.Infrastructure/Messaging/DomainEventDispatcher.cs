using Microsoft.Extensions.DependencyInjection;
using Network.Application.Shared.Events;
using Network.Domain.Events;
using System.Reflection;

namespace Network.Infrastructure.Messaging;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    /// <summary>
    /// Resolve and invoke all pre-save handlers registered for the concrete event type
    /// </summary>
    public async Task DispatchPreSaveAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await InvokeHandlers(typeof(IDomainEventHandler<>), "HandleAsync", domainEvent, cancellationToken);
    }

    /// <summary>
    /// Resolve and invoke all post-save integration event publishers for the concrete event type
    /// </summary>
    public async Task DispatchPostSaveAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await InvokeHandlers(typeof(IIntegrationEventPublisher<>), "PublishAsync", domainEvent, cancellationToken);
    }

    private async Task InvokeHandlers(Type openGenericType, string methodName, IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Type handlerType = openGenericType.MakeGenericType(domainEvent.GetType());
        MethodInfo handleMethod = handlerType.GetMethod(methodName)!;

        foreach (object? handler in serviceProvider.GetServices(handlerType))
        {
            if (handler is null) continue;
            await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
        }
    }
}
