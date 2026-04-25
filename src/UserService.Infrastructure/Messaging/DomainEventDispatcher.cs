using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Shared.Events;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging;

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
