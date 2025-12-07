namespace UserService.Application.Shared.Events;

public interface IIntegrationEvent
{
    public DateTime OccurredOn { get; }
    public Guid Id { get; }
}

public abstract record IntegrationEvent : IIntegrationEvent
{
    public DateTime OccurredOn { get; } = DateTime.Now;
    public Guid Id { get; } = Guid.NewGuid();
}
