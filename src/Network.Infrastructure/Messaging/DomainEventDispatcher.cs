using Microsoft.Extensions.DependencyInjection;
using Network.Application.Shared.Events;
using Network.Domain.Events;
using System.Reflection;

namespace Network.Infrastructure.Messaging;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    /// <summary>
    /// Resolve and invoke all handlers registered for the concrete event type
    /// </summary>
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        MethodInfo handleMethod = handlerType.GetMethod("HandleAsync")!;

        foreach (object? handler in serviceProvider.GetServices(handlerType))
        {
            if (handler is null) continue;
            await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
        }
    }
}
