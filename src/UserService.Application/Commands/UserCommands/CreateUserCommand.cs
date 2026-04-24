using UserService.Application.Dtos.UserDtos;
using UserService.Application.Dtos.UserDtos.Helpers;

namespace UserService.Application.Commands.UserCommands;

public record CreateUserCommand : ICommand
{
    public required UserDto UserDto { get; init; }
}

/// <summary>
/// Create a new user in the database
/// </summary>
public interface ICreateUserCommandHandler : ICommandHandler<CreateUserCommand> { }

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    ICheckUserAddressService checkAddressService,
    IIntegrationEventPublisher eventPublisher
) : ICreateUserCommandHandler
{
    public async Task<Result> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        Result<User> user = UserDtoHelper.ToUser(request.UserDto);

        if (user.IsFailed) return Result.Fail(user.Errors);

        string userAddress = user.Value.Address.AddressLine;

        // Call address service for validation
        bool isAddressValid = await checkAddressService.Check(userAddress, cancellationToken);

        if (!isAddressValid) return Result.Fail("Address is not valid");

        Result<User> createdUser = await userRepository.CreateUser(user.Value);

        if (createdUser.IsFailed) return Result.Fail(createdUser.Errors);

        UserCreatedEvent userCreatedEvent = new()
        {
            UserId = user.Value.Id,
        };

        // Publish an user created event so other services know
        await eventPublisher.PublishAsync(userCreatedEvent);

        return Result.Ok();
    }
}

public record UserCreatedEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
}