using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserDeletedDomainEventHandler(
    IAuditLogRepository auditLogRepository)
    : IPreSavedDomainEventHandler<UserDeletedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is deleted — committed atomically
    /// </summary>
    public async Task HandleAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, AuditLogEventType.UserDeleted);
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value);
    }
}
