namespace UserService.Application.Commands.UserCommands;

public record CreateUserCommand : IRequest<Result<UserDto>>
{
    public UserDto UserDto { get; set; } = new();
}

public class CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository, 
    ICheckUserAddressService checkAddressService) : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICheckUserAddressService _checkAddressService = checkAddressService;

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Result<User> user = _mapper.MapWithResult<User, UserDto>(request.UserDto);

        if (user.IsFailed)
        {
            return Result.Fail(user.Errors);
        }

        string userAddress = user.Value.Address.AddressLine;

        // Call address service for validation
        bool isAddressValid = await _checkAddressService.Check(userAddress, cancellationToken);

        if (!isAddressValid)
        {
            return Result.Fail("Invalid address provided.");
        }

        Result<User> createdUser = await _userRepository.CreateUser(user.Value);

        if (createdUser.IsFailed)
        {
            return Result.Fail(createdUser.Errors);
        }

        UserDto result = _mapper.Map<UserDto>(createdUser);

        return Result.Ok(result);
    }
}