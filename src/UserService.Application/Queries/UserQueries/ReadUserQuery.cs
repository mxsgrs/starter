using UserService.Application.Cqrs;

namespace UserService.Application.Queries.UserQueries;

public interface IReadUserQueryHandler : IQueryByIdHandler<Result<UserDto>> { }

public class ReadUserQueryHandler(IMapper mapper, IUserRepository userRepository) : IReadUserQueryHandler
{
    public async Task<Result<UserDto>> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Result<User> user = await userRepository.ReadUser(id);

        if (user.IsFailed) return Result.Fail(user.Errors);

        UserDto result = mapper.Map<UserDto>(user.Value);

        return Result.Ok(result);
    }
}