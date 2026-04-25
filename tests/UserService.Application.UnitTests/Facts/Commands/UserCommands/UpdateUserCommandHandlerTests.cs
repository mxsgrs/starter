using UserService.Application.Commands.UserCommands;
using UserService.Application.Dtos.UserDtos;
using UserService.Application.Shared.Events;

namespace UserService.Application.UnitTests.Facts.Commands.UserCommands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICheckUserAddressService> _mockCheckUserAddressService;
    private readonly Mock<IDomainEventPublisher> _mockDomainEventPublisher;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCheckUserAddressService = new Mock<ICheckUserAddressService>();
        _mockDomainEventPublisher = new Mock<IDomainEventPublisher>();
        _handler = new UpdateUserCommandHandler(
            _mockUserRepository.Object,
            _mockCheckUserAddressService.Object,
            _mockDomainEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenValidInput()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockCheckUserAddressService.Setup(m => m.Check(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(user.Id))
            .ReturnsAsync(Result.Ok(user));

        _mockUserRepository.Setup(repo => repo.SaveChanges())
            .ReturnsAsync(Result.Ok());

        UpdateUserCommand command = new(user.Id, userWriteDto);

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUserRepository.Verify(repo => repo.SaveChanges(), Times.Once);
        _mockDomainEventPublisher.Verify(p => p.PublishAsync(It.IsAny<UserUpdatedDomainEvent>()), Times.Once);
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
        _mockUserRepository.Verify(repo => repo.SaveChanges(), Times.Never);
        _mockDomainEventPublisher.Verify(p => p.PublishAsync(It.IsAny<UserUpdatedDomainEvent>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenSaveChangesFails()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockCheckUserAddressService.Setup(m => m.Check(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(user.Id))
            .ReturnsAsync(Result.Ok(user));

        _mockUserRepository.Setup(repo => repo.SaveChanges())
            .ReturnsAsync(Result.Fail("Repository error"));

        UpdateUserCommand command = new(user.Id, userWriteDto);

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.False(result.IsSuccess);
        _mockDomainEventPublisher.Verify(p => p.PublishAsync(It.IsAny<UserUpdatedDomainEvent>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenAddressIsInvalid()
    {
        // Arrange
        UserWriteDto userWriteDto = new UserWriteDtoBuilder().Build();
        User user = new UserBuilder().Build();

        _mockUserRepository.Setup(repo => repo.ReadTrackedUser(user.Id))
            .ReturnsAsync(Result.Ok(user));

        _mockCheckUserAddressService.Setup(m => m.Check(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        UpdateUserCommand command = new(user.Id, userWriteDto);

        // Act
        Result result = await _handler.HandleAsync(command, default);

        // Assert
        Assert.False(result.IsSuccess);
        _mockUserRepository.Verify(repo => repo.SaveChanges(), Times.Never);
        _mockDomainEventPublisher.Verify(p => p.PublishAsync(It.IsAny<UserUpdatedDomainEvent>()), Times.Never);
    }
}
