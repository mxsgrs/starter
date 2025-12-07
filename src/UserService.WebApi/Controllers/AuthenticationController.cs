using UserService.Application.Queries.AuthQueries;

namespace UserService.WebApi.Controllers;

public class AuthenticationController(IGenerateTokenQueryHandler generateTokenQueryHandler) : UserServiceControllerBase
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
