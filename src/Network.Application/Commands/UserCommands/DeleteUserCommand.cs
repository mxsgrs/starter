using Network.Application.Shared.Cqrs;

namespace Network.Application.Commands.UserCommands;

/// <summary>
/// Delete a user from the database by ID.
/// </summary>
public interface IDeleteUserCommandHandler : ICommandByIdHandler { }

public class DeleteUserCommandHandler(IUserRepository userRepository) : IDeleteUserCommandHandler
{
    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<User> trackedUser = await userRepository.ReadTrackedUser(id);

        if (trackedUser.IsFailed) return Result.Fail(trackedUser.Errors);

        trackedUser.Value.Delete();

        return await userRepository.DeleteUser(id);
    }
}
