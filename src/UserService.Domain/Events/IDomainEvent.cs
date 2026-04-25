namespace UserService.Domain.Events;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime CreatedOn { get; }
}
