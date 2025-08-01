using MassTransit;

namespace UserService.Domain.Aggregates.UserAggregate;

public record UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; set; }
}

public interface IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid Id { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.Now;
    public Guid Id { get; } = Guid.NewGuid();
}
public class UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger) : IConsumer<UserCreatedEvent>
{
    public Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        logger.LogInformation("Received the user created event {UserCreatedEvent}", context.Message);

        return Task.CompletedTask;
    }
}
