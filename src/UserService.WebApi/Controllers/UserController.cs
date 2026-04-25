using UserService.Application.Dtos.UserDtos;

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

        Result<Guid> result = await createUserCommandHandler.HandleAsync(command);

        return CorrespondingStatus(result);
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
