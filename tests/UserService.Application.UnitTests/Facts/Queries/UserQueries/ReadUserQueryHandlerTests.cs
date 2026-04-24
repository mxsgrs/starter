using Mapster;
using UserService.Application.Dtos.UserDtos;
using UserService.Application.Queries.UserQueries;

namespace UserService.Application.UnitTests.Facts.Queries.UserQueries;

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

        _mockUserRepository.Setup(repo => repo.ReadUser(userId)).ReturnsAsync(user);

        // Act
        Result<UserDto> result = await _handler.HandleAsync(userId, default);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
        Assert.Equal(user.FirstName, result.Value.FirstName);

        _mockUserRepository.Verify(repo => repo.ReadUser(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.ReadUser(userId))
            .ReturnsAsync(Result.Fail<User>("User not found"));

        // Act
        Result<UserDto> result = await _handler.HandleAsync(userId, default);

        // Assert
        Assert.True(result.IsFailed);

        _mockUserRepository.Verify(repo => repo.ReadUser(userId), Times.Once);
    }
}
