using UserService.Application.Dtos.UserDtos;
using UserService.Application.Dtos.UserDtos.Helpers;

namespace UserService.Application.Commands.UserCommands;

public record CreateUserCommand(UserWriteDto UserWriteDto) : ICommand;

/// <summary>
/// Create a new user in the database
/// </summary>
public interface ICreateUserCommandHandler : ICommandHandlerResultingGuid<CreateUserCommand> { }

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    ICheckUserAddressService checkAddressService,
    IDomainEventPublisher eventPublisher
) : ICreateUserCommandHandler
{
    public async Task<Result<Guid>> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        Result<User> user = UserDtoHelper.ToUser(Guid.NewGuid(), request.UserWriteDto);

        if (user.IsFailed) return Result.Fail(user.Errors);

        bool isAddressValid = await checkAddressService.Check(user.Value.Address.AddressLine, cancellationToken);

        if (!isAddressValid) return Result.Fail("Address is not valid");

        Result<User> createdUser = await userRepository.CreateUser(user.Value);

        if (createdUser.IsFailed) return Result.Fail(createdUser.Errors);

        await eventPublisher.DispatchAndClearAsync(createdUser.Value);

        return Result.Ok(createdUser.Value.Id);
    }
}