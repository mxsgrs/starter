namespace Starter.Application.Commands.UserCommands;

public record CreateUserCommand : IRequest<UserDto>
{
    public UserDto UserDto { get; set; } = new();
}

public class CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository) 
    : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User user = _mapper.Map<User>(request.UserDto);

        User createdUser = await _userRepository.CreateUser(user);

        UserDto result = _mapper.Map<UserDto>(createdUser);

        return result;
    }
}