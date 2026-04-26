using Network.Application.Dtos.UserDtos;

namespace Network.WebApi.Controllers;

public class UserController : NetworkControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromServices] ICreateUserCommandHandler createUserCommandHandler,
        UserWriteDto userWriteDto
    ) => CorrespondingStatus(await createUserCommandHandler.HandleAsync(new CreateUserCommand(userWriteDto)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ReadUser(
        Guid id,
        [FromServices] IReadUserQueryHandler readUserQueryHandler
    ) => CorrespondingStatus(await readUserQueryHandler.HandleAsync(id));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromServices] IUpdateUserCommandHandler updateUserCommandHandler,
        UserWriteDto userWriteDto
    ) => CorrespondingStatus(await updateUserCommandHandler.HandleAsync(new UpdateUserCommand(id, userWriteDto)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(
        Guid id,
        [FromServices] IDeleteUserCommandHandler deleteUserCommandHandler
    ) => CorrespondingStatus(await deleteUserCommandHandler.HandleAsync(id));
}
