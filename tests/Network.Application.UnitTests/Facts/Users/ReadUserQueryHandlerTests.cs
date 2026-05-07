using Network.Application.Users.Dtos;
using Network.Application.Users.UseCases;

namespace Network.Application.UnitTests.Facts.Users;

public class ReadUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ReadUserQueryHandler _handler;

    public ReadUserQueryHandlerTests()
    {
        UserMapping.Register(TypeAdapterConfig.GlobalSettings);
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new ReadUserQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        User user = new UserBuilder().WithId(userId).Build();

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(userId)).ReturnsAsync(user);

        // Act
        Result<UserDto> result = await _handler.HandleAsync(userId, default);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
        Assert.Equal(user.FirstName, result.Value.FirstName);

        _mockUserRepository.Verify(repo => repo.ReadTrackedUser(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(userId))
            .ReturnsAsync(Result.Fail<User>("User not found"));

        // Act
        Result<UserDto> result = await _handler.HandleAsync(userId, default);

        // Assert
        Assert.True(result.IsFailed);

        _mockUserRepository.Verify(repo => repo.ReadTrackedUser(userId), Times.Once);
    }
}
