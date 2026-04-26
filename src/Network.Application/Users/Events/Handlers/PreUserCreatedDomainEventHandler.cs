using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserCreatedDomainEventHandler(
    IUserRepository userRepository,
    IAuditLogRepository auditLogRepository,
    ISecurityNoteRepository securityNoteRepository)
    : IPreSavedDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry and, when the new user is 30 or older, a security note — both committed atomically with the user insert
    /// </summary>
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserCreatedDomainEvent));
        await auditLogRepository.AddAsync(auditLog, cancellationToken);

        Result<User> userResult = await userRepository.ReadTrackedUser(domainEvent.UserId);
        if (userResult.IsSuccess && userResult.Value.Age >= 30)
        {
            SecurityNote note = SecurityNote.Create(domainEvent.UserId, "User age is 30 or above");
            await securityNoteRepository.AddAsync(note, cancellationToken);
        }
    }
}
