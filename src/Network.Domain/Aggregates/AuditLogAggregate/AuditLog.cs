namespace Network.Domain.Aggregates.AuditLogAggregate;

public class AuditLog : AggregateRoot
{
    /// <summary>
    /// Create a new audit log entry for a user event.
    /// </summary>
    public static Result<AuditLog> Create(Guid userId, string eventType) =>
        Result.Ok(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventType = eventType,
            OccurredOn = DateTime.UtcNow
        });

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public DateTime OccurredOn { get; private set; }

    private AuditLog() { } // EF Core
}
