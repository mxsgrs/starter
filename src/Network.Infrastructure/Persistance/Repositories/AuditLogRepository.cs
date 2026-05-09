namespace Network.Infrastructure.Persistance.Repositories;

public class AuditLogRepository(NetworkDbContext dbContext) : IAuditLogRepository
{
    /// <summary>
    /// Stage a new audit log entry to be committed as part of the current transaction
    /// </summary>
    public async Task AddAsync(AuditLog auditLog)
        => await dbContext.AuditLogs.AddAsync(auditLog);
}
