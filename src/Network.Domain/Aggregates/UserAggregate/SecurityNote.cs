namespace Network.Domain.Aggregates.UserAggregate;

public class SecurityNote
{
    /// <summary>
    /// Create a security note for a user whose age is 30 or above
    /// </summary>
    public static SecurityNote Create(Guid userId, string reason) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Reason = reason,
        CreatedOn = DateTime.UtcNow
    };

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime CreatedOn { get; private set; }
}
