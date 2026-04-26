using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserDeletedDomainEventHandler(
    IAuditLogRepository auditLogRepository,
    ISecurityNoteRepository securityNoteRepository)
    : IPreSavedDomainEventHandler<UserDeletedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is deleted and remove their security note if one exists — committed atomically
    /// </summary>
    public async Task HandleAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserDeletedDomainEvent));
        await auditLogRepository.AddAsync(auditLog, cancellationToken);

        SecurityNote? note = await securityNoteRepository.FindByUserIdAsync(domainEvent.UserId, cancellationToken);
        if (note is not null)
            securityNoteRepository.Remove(note);
    }
}
