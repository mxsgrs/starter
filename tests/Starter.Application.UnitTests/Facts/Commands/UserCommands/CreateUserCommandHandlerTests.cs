using AutoMapper;
using Starter.Application.Commands.UserCommands;
using Starter.Application.Dtos;

namespace Starter.Application.UnitTests.Facts.Commands.UserCommands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ICheckUserAddressService> _mockCheckUserAddressService;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCheckUserAddressService = new Mock<ICheckUserAddressService>();
        _handler = new CreateUserCommandHandler(_mockMapper.Object, _mockUserRepository.Object, _mockCheckUserAddressService.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidInput()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            HashedPassword = "hashedPassword",
            Birthday = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Role = Role.User,
            Phone = "+1234567890",
            Address = new UserAddressDto
            {
                AddressLine = "123 Main St",
                City = "Metropolis",
                ZipCode = "12345",
                Country = "USA"
            }
        };

        var user = new User(
            Guid.NewGuid(),
            userDto.EmailAddress,
            userDto.HashedPassword,
            userDto.FirstName,
            userDto.LastName,
            userDto.Birthday,
            userDto.Gender,
            userDto.Role,
            userDto.Phone,
            new Address(userDto.Address.AddressLine, userDto.Address.City, userDto.Address.ZipCode, userDto.Address.Country)
        );

        var createdUser = user;

        _mockMapper.Setup(m => m.Map<User>(userDto)).Returns(user);
        _mockUserRepository.Setup(repo => repo.CreateUser(It.IsAny<User>())).ReturnsAsync(createdUser);
        _mockMapper.Setup(m => m.Map<UserDto>(createdUser)).Returns(userDto);

        var command = new CreateUserCommand { UserDto = userDto };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.Id, result.Id);
        Assert.Equal(userDto.EmailAddress, result.EmailAddress);
        Assert.Equal(userDto.FirstName, result.FirstName);

        _mockMapper.Verify(m => m.Map<User>(userDto), Times.Once);
        _mockUserRepository.Verify(repo => repo.CreateUser(It.Is<User>(u => u.EmailAddress == userDto.EmailAddress)), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(createdUser), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserRepositoryFails()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        var command = new CreateUserCommand { UserDto = userDto };

        _mockMapper.Setup(m => m.Map<User>(userDto)).Throws<InvalidOperationException>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, default));

        _mockMapper.Verify(m => m.Map<User>(userDto), Times.Once);
        _mockUserRepository.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
    }
}

