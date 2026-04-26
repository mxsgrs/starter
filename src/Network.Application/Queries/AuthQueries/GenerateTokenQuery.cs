using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Network.Application.Dtos;
using Network.Application.Shared.Cqrs;
using Network.Domain.Authentication;
using System.Security.Claims;
using System.Text;

namespace Network.Application.Queries.AuthQueries;

public record GenerateTokenQuery(string EmailAddress, string HashedPassword) : IQuery<Result<LoginResponseDto>>;

public interface IGenerateTokenQueryHandler : IQueryHandler<GenerateTokenQuery, Result<LoginResponseDto>> { }

public class GenerateTokenQueryHandler(
    ILogger<GenerateTokenQueryHandler> logger,
    IConfiguration configuration,
    IUserRepository userService
) : IGenerateTokenQueryHandler
{
    public async Task<Result<LoginResponseDto>> HandleAsync(GenerateTokenQuery query, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Processing login request for {emailAddress}", query.EmailAddress);

        Result<User> user = await ValidateUser(query.EmailAddress, query.HashedPassword);

        if (user.IsFailed)
        {
            return Result.Fail($"Login {query.EmailAddress} doesn't exists");
        }

        JsonWebTokenParameters? jwtParameters = configuration
            .GetRequiredSection("JsonWebTokenParameters")
            .Get<JsonWebTokenParameters>()
                ?? throw new Exception("JWT settings are not configured");

        string accessToken = GenerateToken(user.Value.Id, query.EmailAddress, jwtParameters);

        LoginResponseDto dto = new()
        {
            AccessToken = accessToken
        };

        return Result.Ok(dto);
    }

    private async Task<Result<User>> ValidateUser(string emailAddress, string hashedPassword)
    {
        return await userService.ReadUserByCredentials(emailAddress, hashedPassword);
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
