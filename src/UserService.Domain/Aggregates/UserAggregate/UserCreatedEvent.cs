namespace UserService.Domain.Aggregates.UserAggregate;

public record UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; set; }
}
