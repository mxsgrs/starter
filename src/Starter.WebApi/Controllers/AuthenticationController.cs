using Starter.Application.AuthenticationFeatures;

namespace Starter.WebApi.Controllers;

public class AuthenticationController(IJwtService jsonWebTokenService) : StarterControllerBase
{
    private readonly IJwtService _jsonWebTokenService = jsonWebTokenService;

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> Token(HashedLoginRequest hashedLoginRequest)
    {
        LoginResponse result = await _jsonWebTokenService.CreateToken(hashedLoginRequest);

        return Ok(result);
    }
}
