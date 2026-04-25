using UserService.Application.Commands.UserCommands;
using UserService.Application.Dtos.UserDtos;
using UserService.Application.Shared.Events;

namespace UserService.Application.UnitTests.Facts.Commands.UserCommands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICheckUserAddressService> _mockCheckUserAddressService;
    private readonly Mock<IDomainEventPublisher> _mockDomainEventPublisher;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCheckUserAddressService = new Mock<ICheckUserAddressService>();
        _mockDomainEventPublisher = new Mock<IDomainEventPublisher>();
        _handler = new CreateUserCommandHandler(
            _mockUserRepository.Object,
            _mockCheckUserAddressService.Object,
            _mockDomainEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidInput()
    {
        // Arrange
        UserDto userDto = new UserDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockCheckUserAddressService.Setup(m => m.Check(userDto.Address!.AddressLine, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(Result.Ok(user));

        CreateUserCommand command = new() { UserDto = userDto };

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUserRepository.Verify(repo => repo.CreateUser(It.Is<User>(u => u.EmailAddress == userDto.EmailAddress)), Times.Once);
        _mockDomainEventPublisher.Verify(p => p.PublishAsync(It.IsAny<UserCreatedDomainEvent>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenUserRepositoryFails()
    {
        // Arrange
        UserDto userDto = new UserDtoBuilder().Build();

        _mockCheckUserAddressService.Setup(m => m.Check(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(Result.Fail<User>("Repository error"));

        CreateUserCommand command = new() { UserDto = userDto };

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.False(result.IsSuccess);
        _mockDomainEventPublisher.Verify(p => p.PublishAsync(It.IsAny<UserCreatedDomainEvent>()), Times.Never);
    }
}
