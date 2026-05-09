using Network.Application.Users.Dtos.Helpers;

namespace Network.Application.Users.UseCases;

public record UpdateUserCommand(Guid Id, UserWriteDto UserWriteDto) : ICommand;

/// <summary>
/// Update an existing user in the database
/// </summary>
public interface IUpdateUserCommandHandler : ICommandHandler<UpdateUserCommand> { }

public class UpdateUserCommandHandler(
    IUserRepository userRepository
) : IUpdateUserCommandHandler
{
    public async Task<Result> HandleAsync(UpdateUserCommand request)
    {
        Result<User> trackedUser = await userRepository.FindByIdAsync(request.Id);

        if (trackedUser.IsFailed) return Result.Fail(trackedUser.Errors);

        Result updateResult = UserDtoHelper.ApplyUpdate(trackedUser.Value, request.UserWriteDto);

        if (updateResult.IsFailed) return Result.Fail(updateResult.Errors);

        Result savedUser = await userRepository.UpdateAsync(request.Id);

        if (savedUser.IsFailed) return Result.Fail(savedUser.Errors);

        return Result.Ok();
    }
}
