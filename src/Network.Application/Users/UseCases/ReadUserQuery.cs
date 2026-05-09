namespace Network.Application.Users.UseCases;

public interface IReadUserQueryHandler : IQueryByIdHandler<Result<UserDto>> { }

public class ReadUserQueryHandler(IUserRepository userRepository) : IReadUserQueryHandler
{
    public async Task<Result<UserDto>> HandleAsync(Guid id)
    {
        Result<User> user = await userRepository.FindByIdAsync(id);

        if (user.IsFailed) return Result.Fail(user.Errors);

        UserDto dto = user.Value.Adapt<UserDto>();

        return Result.Ok(dto);
    }
}