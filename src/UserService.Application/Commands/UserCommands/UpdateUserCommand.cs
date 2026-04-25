using UserService.Application.Dtos.UserDtos;
using UserService.Application.Dtos.UserDtos.Helpers;

namespace UserService.Application.Commands.UserCommands;

public record UpdateUserCommand(Guid Id, UserWriteDto UserWriteDto) : ICommand;

/// <summary>
/// Update an existing user in the database
/// </summary>
public interface IUpdateUserCommandHandler : ICommandHandler<UpdateUserCommand> { }

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    ICheckUserAddressService checkAddressService,
    IDomainEventPublisher eventPublisher
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

        Result savedUser = await userRepository.SaveChanges();

        if (savedUser.IsFailed) return Result.Fail(savedUser.Errors);

        await eventPublisher.DispatchAndClearAsync(trackedUser.Value);

        return Result.Ok();
    }
}
