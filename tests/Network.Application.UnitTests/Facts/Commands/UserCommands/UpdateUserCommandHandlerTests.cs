using Network.Application.Commands.UserCommands;
using Network.Application.Dtos.UserDtos;

namespace Network.Application.UnitTests.Facts.Commands.UserCommands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new UpdateUserCommandHandler(
            _mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenValidInput()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(user.Id))
            .ReturnsAsync(Result.Ok(user));

        _mockUserRepository.Setup(repo => repo.UpdateUser(user.Id))
            .ReturnsAsync(Result.Ok());

        UpdateUserCommand command = new(user.Id, userWriteDto);

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUserRepository.Verify(repo => repo.UpdateUser(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        Guid unknownId = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(unknownId))
            .ReturnsAsync(Result.Fail<User>("User not found"));

        UpdateUserCommand command = new(unknownId, userWriteDto);

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.False(result.IsSuccess);
        _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenUpdateUserFails()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(user.Id))
            .ReturnsAsync(Result.Ok(user));

        _mockUserRepository.Setup(repo => repo.UpdateUser(user.Id))
            .ReturnsAsync(Result.Fail("Repository error"));

        UpdateUserCommand command = new(user.Id, userWriteDto);

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.False(result.IsSuccess);
    }

}
