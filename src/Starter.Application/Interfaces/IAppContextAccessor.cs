using Starter.Domain.Authentication;

namespace Starter.Application.Interfaces;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}