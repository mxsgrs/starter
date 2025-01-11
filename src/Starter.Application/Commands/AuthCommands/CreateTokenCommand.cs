using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Starter.Domain.Authentication;
using Starter.Domain.Exceptions;
using System.Security.Claims;
using System.Text;

namespace Starter.Application.Commands.AuthCommands;

public record CreateTokenCommand(string EmailAddress, string HashedPassword) : IRequest<LoginResponseDto>;

public class CreateTokenCommandHandler(ILogger<CreateTokenCommandHandler> logger, IConfiguration configuration, 
    IUserRepository userService) : IRequestHandler<CreateTokenCommand, LoginResponseDto>
{
    private readonly ILogger<CreateTokenCommandHandler> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly IUserRepository _userService = userService;

    public async Task<LoginResponseDto> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing login request for {emailAddress}", request.EmailAddress);

        // Validate user
        Guid userId = await ValidateUser(request.EmailAddress, request.HashedPassword);

        // Retrieve JWT parameters
        JsonWebTokenParameters jwtParameters = _configuration
            .GetRequiredSection("JsonWebTokenParameters")
            .Get<JsonWebTokenParameters>()
                ?? throw new Exception("JWT settings are not configured");

        // Generate token
        string accessToken = GenerateToken(userId, request.EmailAddress, jwtParameters);

        // Create response
        return new LoginResponseDto
        {
            AccessToken = accessToken
        };
    }

    private async Task<Guid> ValidateUser(string emailAddress, string hashedPassword)
    {
        try
        {
            User user = await _userService.ReadUser(emailAddress, hashedPassword);
            return user.Id;
        }
        catch (Exception)
        {
            throw new UnauthorizedException();
        }
    }

    private static string GenerateToken(Guid userId, string emailAddress, JsonWebTokenParameters jwtParameters)
    {
        byte[] encodedKey = Encoding.ASCII.GetBytes(jwtParameters.Key);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, emailAddress)
            ]),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = jwtParameters.Issuer,
            Audience = jwtParameters.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(encodedKey),
                SecurityAlgorithms.HmacSha512Signature)
        };

        JsonWebTokenHandler handler = new()
        {
            SetDefaultTimesOnTokenCreation = false
        };

        return handler.CreateToken(tokenDescriptor);
    }
}
