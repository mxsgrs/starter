using Network.Application.Users.Dtos;
using Network.Application.Users.UseCases;

namespace Network.Application.UnitTests.Facts.Users;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new CreateUserCommandHandler(
            _mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidInput()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Ok(user));

        CreateUserCommand command = new(userWriteDto);

        // Act
        Result<Guid> result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value);
        _mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u => u.EmailAddress == userWriteDto.EmailAddress)), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenUserRepositoryFails()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();

        _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Fail<User>("Repository error"));

        CreateUserCommand command = new(userWriteDto);

        // Act
        Result<Guid> result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
