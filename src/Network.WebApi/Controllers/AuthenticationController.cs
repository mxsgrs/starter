using Network.Application.Authentication.Dtos;

namespace Network.WebApi.Controllers;

public class AuthenticationController(IGenerateTokenQueryHandler generateTokenQueryHandler) : NetworkControllerBase
{
    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> Token(HashedLoginRequestDto hashedLoginRequest)
    {
        GenerateTokenQuery command = new(hashedLoginRequest.EmailAddress, hashedLoginRequest.HashedPassword);

        Result<LoginResponseDto> result = await generateTokenQueryHandler.HandleAsync(command);

        return CorrespondingStatus(result);
    }
}
