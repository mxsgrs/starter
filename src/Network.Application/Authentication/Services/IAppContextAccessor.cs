using Network.Domain.Authentication;

namespace Network.Application.Authentication.Services;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}