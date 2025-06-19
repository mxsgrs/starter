namespace UserService.WebApi.Controllers;

public class AuthenticationController(ISender sender) : UserServiceControllerBase
{
    private readonly ISender _sender = sender;

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> Token(HashedLoginRequestDto hashedLoginRequest)
    {
        CreateTokenCommand command = new(hashedLoginRequest.EmailAddress, hashedLoginRequest.HashedPassword);

        Result<LoginResponseDto> result = await _sender.Send(command);

        return CorrespondingStatus(result);
    }
}
