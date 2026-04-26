using Network.Domain.Authentication;

namespace Network.Application.Shared.Interfaces;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}