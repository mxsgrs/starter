using MassTransit;
using Network.Application.IntegrationEvents;
using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Persistance;

namespace Network.Infrastructure.Messaging;

public class UserCreatedDomainEventHandler(UserDbContext dbContext, IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry and publish an integration event when a user is created
    /// </summary>
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserCreatedDomainEvent));
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
        await publishEndpoint.Publish(new UserCreatedIntegrationEvent(domainEvent.UserId), cancellationToken);
    }
}
