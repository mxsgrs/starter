namespace UserService.WebApi.Controllers;

public class UserController : UserServiceControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromServices] ICreateUserCommandHandler createUserCommandHandler,
        UserDto userDto
    )
    {
        CreateUserCommand command = new()
        {
            UserDto = userDto
        };

        await createUserCommandHandler.HandleAsync(command);

        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ReadUser(
        Guid id,
        [FromServices] IReadUserQueryHandler readUserQueryHandler
    )
    {
        Result<UserDto> resultDto = await readUserQueryHandler.HandleAsync(id);

        return CorrespondingStatus(resultDto);
    }
}
