namespace Starter.WebApi.Controllers;

public class AuthenticationController(ISender sender) : StarterControllerBase
{
    private readonly ISender _sender = sender;

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> Token(HashedLoginRequestDto hashedLoginRequest)
    {
        CreateTokenCommand command = new(hashedLoginRequest.EmailAddress, hashedLoginRequest.HashedPassword);

        LoginResponseDto result = await _sender.Send(command);

        return Ok(result);
    }
}
