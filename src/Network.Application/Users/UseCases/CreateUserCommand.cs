using Network.Application.Users.Dtos.Helpers;

namespace Network.Application.Users.UseCases;

public record CreateUserCommand(UserWriteDto UserWriteDto) : ICommand;

/// <summary>
/// Create a new user in the database
/// </summary>
public interface ICreateUserCommandHandler : ICommandHandlerResultingGuid<CreateUserCommand> { }

public class CreateUserCommandHandler(
    IUserRepository userRepository
) : ICreateUserCommandHandler
{
    public async Task<Result<Guid>> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        Result<User> user = UserDtoHelper.ToUser(Guid.NewGuid(), request.UserWriteDto);

        if (user.IsFailed) return Result.Fail(user.Errors);

        Result<User> createdUser = await userRepository.AddAsync(user.Value);

        if (createdUser.IsFailed) return Result.Fail(createdUser.Errors);

        return Result.Ok(createdUser.Value.Id);
    }
}