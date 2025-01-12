namespace Starter.Application.Commands.UserCommands;

public record CreateUserCommand : IRequest<UserDto>
{
    public UserDto UserDto { get; set; } = new();
}

public class CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository, 
    ICheckUserAddressService checkAddressService) : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICheckUserAddressService _checkAddressService = checkAddressService;

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User user = _mapper.Map<User>(request.UserDto);

        string userAddress = user.Address.AddressLine;

        bool isAddressValid = await _checkAddressService.Check(userAddress, cancellationToken);

        if (!isAddressValid)
        {
            throw new InvalidUserAddressException(userAddress);
        }

        User createdUser = await _userRepository.CreateUser(user);

        UserDto result = _mapper.Map<UserDto>(createdUser);

        return result;
    }
}