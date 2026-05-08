using Network.Application.Users.UseCases;

namespace Network.Application.UnitTests.Facts.Users;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenValidInput()
    {
        // Arrange
        User user = new UserBuilder().Build();

        _mockUserRepository.Setup(repo => repo.FindByIdAsync(user.Id))
            .ReturnsAsync(Result.Ok(user));

        _mockUserRepository.Setup(repo => repo.RemoveAsync(user.Id))
            .ReturnsAsync(Result.Ok());

        // Act
        Result result = await _handler.HandleAsync(user.Id, default);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUserRepository.Verify(repo => repo.RemoveAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        Guid unknownId = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.FindByIdAsync(unknownId))
            .ReturnsAsync(Result.Fail<User>("User not found"));

        // Act
        Result result = await _handler.HandleAsync(unknownId, default);

        // Assert
        Assert.False(result.IsSuccess);
        _mockUserRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenDeleteFails()
    {
        // Arrange
        User user = new UserBuilder().Build();

        _mockUserRepository.Setup(repo => repo.FindByIdAsync(user.Id))
            .ReturnsAsync(Result.Ok(user));

        _mockUserRepository.Setup(repo => repo.RemoveAsync(user.Id))
            .ReturnsAsync(Result.Fail("Repository error"));

        // Act
        Result result = await _handler.HandleAsync(user.Id, default);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
