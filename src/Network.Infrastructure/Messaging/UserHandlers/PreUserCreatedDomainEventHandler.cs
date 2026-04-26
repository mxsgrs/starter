using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Persistance;

namespace Network.Infrastructure.Messaging.UserHandlers;

public class PreUserCreatedDomainEventHandler(UserDbContext dbContext)
    : IPreSavedDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry and, when the new user is 30 or older, a security note — both committed atomically with the user insert
    /// </summary>
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserCreatedDomainEvent));
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);

        User? user = dbContext.Users.Local.FirstOrDefault(u => u.Id == domainEvent.UserId);
        if (user is not null && user.Age >= 30)
        {
            SecurityNote note = SecurityNote.Create(domainEvent.UserId, "User age is 30 or above");
            await dbContext.SecurityNotes.AddAsync(note, cancellationToken);
        }
    }
}
