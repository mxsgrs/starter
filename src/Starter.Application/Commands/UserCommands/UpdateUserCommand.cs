namespace Starter.Application.Commands.UserCommands;

public record UpdateUserCommand : IRequest<UserDto>
{
    public UserDto UserDto { get; set; } = new();
}

public class UpdateUserCommandHandler(IMapper mapper, IUserRepository userRepository) 
    : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User user = _mapper.Map<User>(request.UserDto);

        User createdUser = await _userRepository.UpdateUser(user.Id, user);

        UserDto result = _mapper.Map<UserDto>(createdUser);

        return result;
    }
}