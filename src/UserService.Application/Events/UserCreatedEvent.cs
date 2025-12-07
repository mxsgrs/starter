namespace UserService.Application.Events;

public record UserCreatedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
}
