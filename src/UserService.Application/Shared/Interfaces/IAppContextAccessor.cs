using UserService.Domain.Authentication;

namespace UserService.Application.Shared.Interfaces;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}