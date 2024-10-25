using Starter.Domain.Authentication;

namespace Starter.Application.AuthenticationFeatures;

public interface IJwtService
{
    Task<LoginResponse> CreateToken(HashedLoginRequest hashedLoginRequest);
}