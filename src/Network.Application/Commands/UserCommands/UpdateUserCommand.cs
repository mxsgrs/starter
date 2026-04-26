using Network.Application.Dtos.UserDtos;
using Network.Application.Dtos.UserDtos.Helpers;
using Network.Application.Shared.Cqrs;

namespace Network.Application.Commands.UserCommands;

public record UpdateUserCommand(Guid Id, UserWriteDto UserWriteDto) : ICommand;

/// <summary>
/// Update an existing user in the database
/// </summary>
public interface IUpdateUserCommandHandler : ICommandHandler<UpdateUserCommand> { }

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    ICheckUserAddressService checkAddressService
) : IUpdateUserCommandHandler
{
    public async Task<Result> HandleAsync(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        Result<User> trackedUser = await userRepository.ReadTrackedUser(request.Id);

        if (trackedUser.IsFailed) return Result.Fail(trackedUser.Errors);

        Result updateResult = UserDtoHelper.ApplyUpdate(trackedUser.Value, request.UserWriteDto);

        if (updateResult.IsFailed) return Result.Fail(updateResult.Errors);

        bool isAddressValid = await checkAddressService.Check(trackedUser.Value.Address.AddressLine, cancellationToken);

        if (!isAddressValid) return Result.Fail("Address is not valid");

        Result savedUser = await userRepository.UpdateUser(request.Id);

        if (savedUser.IsFailed) return Result.Fail(savedUser.Errors);

        return Result.Ok();
    }
}
