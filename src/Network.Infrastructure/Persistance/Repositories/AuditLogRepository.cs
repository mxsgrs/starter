using Network.Domain.Aggregates.UserAggregate;

namespace Network.Infrastructure.Persistance.Repositories;

public class AuditLogRepository(UserDbContext dbContext) : IAuditLogRepository
{
    /// <summary>
    /// Stage a new audit log entry to be committed as part of the current transaction
    /// </summary>
    public async Task AddAsync(UserAuditLog auditLog, CancellationToken cancellationToken)
        => await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
}
