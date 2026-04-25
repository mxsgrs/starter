namespace UserService.Domain.Events;

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}
