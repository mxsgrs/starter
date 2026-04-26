using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserUpdatedDomainEventHandler(
    IUserRepository userRepository,
    IAuditLogRepository auditLogRepository,
    ISecurityNoteRepository securityNoteRepository)
    : IPreSavedDomainEventHandler<UserUpdatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is updated, and create or delete their security note based on age — committed atomically
    /// </summary>
    public async Task HandleAsync(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserUpdatedDomainEvent));
        await auditLogRepository.AddAsync(auditLog, cancellationToken);

        Result<User> userResult = await userRepository.ReadTrackedUser(domainEvent.UserId);
        if (!userResult.IsSuccess) return;

        User user = userResult.Value;
        SecurityNote? existingNote = await securityNoteRepository.FindByUserIdAsync(domainEvent.UserId, cancellationToken);

        if (user.Age >= 30 && existingNote is null)
        {
            SecurityNote note = SecurityNote.Create(domainEvent.UserId, "User age is 30 or above");
            await securityNoteRepository.AddAsync(note, cancellationToken);
        }
        else if (user.Age < 30 && existingNote is not null)
        {
            securityNoteRepository.Remove(existingNote);
        }
    }
}
