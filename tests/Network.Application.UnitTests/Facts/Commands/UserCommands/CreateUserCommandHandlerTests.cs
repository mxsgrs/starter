using Network.Application.Commands.UserCommands;
using Network.Application.Dtos.UserDtos;

namespace Network.Application.UnitTests.Facts.Commands.UserCommands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICheckUserAddressService> _mockCheckUserAddressService;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCheckUserAddressService = new Mock<ICheckUserAddressService>();
        _handler = new CreateUserCommandHandler(
            _mockUserRepository.Object,
            _mockCheckUserAddressService.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidInput()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockCheckUserAddressService.Setup(m => m.Check(userWriteDto.Address!.AddressLine, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(Result.Ok(user));

        CreateUserCommand command = new(userWriteDto);

        // Act
        Result<Guid> result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value);
        _mockUserRepository.Verify(repo => repo.CreateUser(It.Is<User>(u => u.EmailAddress == userWriteDto.EmailAddress)), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenUserRepositoryFails()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();

        _mockCheckUserAddressService.Setup(m => m.Check(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(Result.Fail<User>("Repository error"));

        CreateUserCommand command = new(userWriteDto);

        // Act
        Result<Guid> result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
