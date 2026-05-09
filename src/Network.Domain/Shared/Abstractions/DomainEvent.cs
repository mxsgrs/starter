namespace Network.Domain.Shared.Abstractions;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime CreatedOn { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}
