namespace Starter.WebApi.Controllers;

public class UserController(ISender sender) : StarterControllerBase
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

        UserDto resultDto = await _sender.Send(command);

        return Ok(resultDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ReadUser(Guid id)
    {
        ReadUserQuery query = new()
        {
            Id = id
        };

        UserDto resultDto = await _sender.Send(query);

        return Ok(resultDto);
    }
}
