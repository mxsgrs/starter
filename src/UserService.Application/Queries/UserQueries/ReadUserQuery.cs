namespace UserService.Application.Queries.UserQueries;

public record ReadUserQuery : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
}

public class ReadUserQueryHandler(IMapper mapper, IUserRepository userRepository)
    : IRequestHandler<ReadUserQuery, Result<UserDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<UserDto>> Handle(ReadUserQuery request, CancellationToken cancellationToken)
    {
        Result<User> user = await _userRepository.ReadUser(request.Id);

        if (user.IsFailed)
        {
            return Result.Fail(user.Errors);
        }

        UserDto result = _mapper.Map<UserDto>(user);

        return Result.Ok(result);
    }
}