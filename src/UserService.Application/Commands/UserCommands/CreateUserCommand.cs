using UserService.Application.Dtos.UserDtos;

namespace UserService.Application.Commands.UserCommands;

public record CreateUserCommand : ICommand
{
    public required UserDto UserDto { get; init; }
}

public interface ICreateUserCommandHandler : ICommandHandler<CreateUserCommand> { }

public class CreateUserCommandHandler(
    IMapper mapper,
    IUserRepository userRepository,
    ICheckUserAddressService checkAddressService,
    IIntegrationEventPublisher eventPublisher
) : ICreateUserCommandHandler
{
    public async Task HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        Result<User> user = mapper.TryMap<User, UserDto>(request.UserDto);

        if (user.IsFailed) return;

        string userAddress = user.Value.Address.AddressLine;

        // Call address service for validation
        bool isAddressValid = await checkAddressService.Check(userAddress, cancellationToken);

        if (!isAddressValid) return;

        Result<User> createdUser = await userRepository.CreateUser(user.Value);

        if (createdUser.IsFailed) return;

        UserCreatedEvent userCreatedEvent = new()
        {
            UserId = user.Value.Id,
        };

        // Publish an user created event so other services know
        await eventPublisher.PublishAsync(userCreatedEvent);
    }
}
public record UserCreatedEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
}