namespace Starter.Application.Queries.UserQueries;

public record ReadUserQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }
}

public class ReadUserQueryHandler(IMapper mapper, IUserRepository userRepository)
    : IRequestHandler<ReadUserQuery, UserDto>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> Handle(ReadUserQuery request, CancellationToken cancellationToken)
    {
        User user = await _userRepository.ReadUser(request.Id);

        UserDto result = _mapper.Map<UserDto>(user);

        return result;
    }
}