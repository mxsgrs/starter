using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserUpdatedDomainEventHandler(
    IAuditLogRepository auditLogRepository)
    : IPreSavedDomainEventHandler<UserUpdatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is updated — committed atomically
    /// </summary>
    public async Task HandleAsync(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, AuditLogEventType.UserUpdated);
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value);
    }
}
