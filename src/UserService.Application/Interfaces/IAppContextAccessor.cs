using UserService.Domain.Authentication;

namespace UserService.Application.Interfaces;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}