namespace Network.Application.Users.UseCases;

/// <summary>
/// Delete a user from the database by ID.
/// </summary>
public interface IDeleteUserCommandHandler : ICommandByIdHandler { }

public class DeleteUserCommandHandler(IUserRepository userRepository) : IDeleteUserCommandHandler
{
    public async Task<Result> HandleAsync(Guid id)
    {
        Result<User> trackedUser = await userRepository.FindByIdAsync(id);

        if (trackedUser.IsFailed) return Result.Fail(trackedUser.Errors);

        trackedUser.Value.Delete();

        return await userRepository.RemoveAsync(id);
    }
}
