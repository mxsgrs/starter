using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UserService.Domain.Authentication;

namespace UserService.Application.Commands.AuthCommands;

public record CreateTokenCommand(string EmailAddress, string HashedPassword) : IRequest<Result<LoginResponseDto>>;

public class CreateTokenCommandHandler(ILogger<CreateTokenCommandHandler> logger, IConfiguration configuration, 
    IUserRepository userService) : IRequestHandler<CreateTokenCommand, Result<LoginResponseDto>>
{
    private readonly ILogger<CreateTokenCommandHandler> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly IUserRepository _userService = userService;

    public async Task<Result<LoginResponseDto>> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing login request for {emailAddress}", request.EmailAddress);

        Result<User> user = await ValidateUser(request.EmailAddress, request.HashedPassword);

        if(user.IsFailed)
        {
            return Result.Fail($"Login {request.EmailAddress} doesn't exists");
        }

        JsonWebTokenParameters? jwtParameters = _configuration
            .GetRequiredSection("JsonWebTokenParameters")
            .Get<JsonWebTokenParameters>()
                ?? throw new Exception("JWT settings are not configured");

        string accessToken = GenerateToken(user.Value.Id, request.EmailAddress, jwtParameters);

        LoginResponseDto dto = new()
        {
            AccessToken = accessToken
        };

        return Result.Ok(dto);
    }

    private async Task<Result<User>> ValidateUser(string emailAddress, string hashedPassword)
    {
        return await _userService.ReadUser(emailAddress, hashedPassword);
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
