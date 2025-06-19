using AutoMapper;
using UserService.Application.Commands.UserCommands;
using UserService.Application.Dtos;

namespace UserService.Application.UnitTests.Facts.Commands.UserCommands;

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
        UserDto userDto = new()
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

        User user = new(
            Guid.NewGuid(),
            userDto.EmailAddress,
            userDto.HashedPassword,
            userDto.FirstName,
            userDto.LastName,
            userDto.Birthday,
            userDto.Gender,
            userDto.Role,
            userDto.Phone,
            new Address
            (
                userDto.Address.AddressLine, 
                userDto.Address.City,
                userDto.Address.ZipCode,
                userDto.Address.Country
            )
        );

        User createdUser = user;

        _mockMapper.Setup(m => m.Map<User>(userDto))
            .Returns(user);

        _mockUserRepository.Setup(repo => repo.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        _mockMapper.Setup(m => m.Map<UserDto>(createdUser))
            .Returns(userDto);

        _mockCheckUserAddressService.Setup(m => m.Check(userDto.Address.AddressLine, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        CreateUserCommand command = new() { UserDto = userDto };

        // Act
        Result<UserDto> result = await _handler.Handle(command, default);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userDto.Id, result.Value.Id);
        Assert.Equal(userDto.EmailAddress, result.Value.EmailAddress);
        Assert.Equal(userDto.FirstName, result.Value.FirstName);

        _mockMapper.Verify(m => m.Map<User>(userDto), Times.Once);

        _mockUserRepository.Verify(repo => repo.CreateUser(It.Is<User>(u => u.EmailAddress == userDto.EmailAddress)), Times.Once);

        _mockMapper.Verify(m => m.Map<UserDto>(createdUser), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnFailure_WhenUserRepositoryFails()
    {
        // Arrange
        UserDto userDto = new()
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        CreateUserCommand command = new() 
        {
            UserDto = userDto
        };

        _mockMapper.Setup(m => m.Map<User>(userDto))
            .Throws<InvalidOperationException>();

        // Act
        Result<UserDto> result = await _handler.Handle(command, default);

        // Assert
        Assert.True(result.IsFailed);

        _mockMapper.Verify(m => m.Map<User>(userDto), Times.Once);

        _mockUserRepository.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
    }
}

