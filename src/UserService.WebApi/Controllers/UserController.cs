namespace UserService.WebApi.Controllers;

public class UserController(ISender sender) : UserServiceControllerBase
{
    private readonly ISender _sender = sender;

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        CreateUserCommand command = new()
        {
            UserDto = userDto
        };

        Result<UserDto> resultDto = await _sender.Send(command);

        return CorrespondingStatus(resultDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ReadUser(Guid id)
    {
        ReadUserQuery query = new()
        {
            Id = id
        };

        Result<UserDto> resultDto = await _sender.Send(query);

        return CorrespondingStatus(resultDto);
    }
}
