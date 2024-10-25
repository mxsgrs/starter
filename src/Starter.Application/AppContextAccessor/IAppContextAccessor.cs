using Starter.Domain.Authentication;

namespace Starter.Application.AppContextAccessor;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}