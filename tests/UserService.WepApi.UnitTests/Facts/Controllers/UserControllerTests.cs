using UserService.Application.Commands.UserCommands;
using UserService.Application.Dtos;
using UserService.Application.Queries.UserQueries;
using UserService.Domain.Aggregates.UserAggregate;

namespace UserService.WepApi.UnitTests.Facts.Controllers;

public class UserControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new UserController(_senderMock.Object);
    }

    [Fact]
    public async Task CreateUser_ReturnsOkResult_WithCreatedUserDto()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            HashedPassword = "hashed_password",
            FirstName = "John",
            LastName = "Doe",
            Birthday = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Role = Role.User,
            Phone = "+1234567890",
            Address = new UserAddressDto
            {
                AddressLine = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Country = "Test Country"
            }
        };

        CreateUserCommand createUserCommand = new() { UserDto = userDto };

        _senderMock.Setup(s => s.Send(It.IsAny<CreateUserCommand>(), default))
            .ReturnsAsync(userDto);

        // Act
        IActionResult result = await _controller.CreateUser(userDto);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userDto, okResult.Value);

        _senderMock.Verify(s => s.Send(It.Is<CreateUserCommand>(c => c.UserDto == userDto), default), Times.Once);
    }

    [Fact]
    public async Task ReadUser_ReturnsOkResult_WithUserDto()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        UserDto userDto = new()
        {
            Id = userId,
            EmailAddress = "test@example.com",
            HashedPassword = "hashed_password",
            FirstName = "John",
            LastName = "Doe",
            Birthday = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Role = Role.User,
            Phone = "+1234567890",
            Address = new UserAddressDto
            {
                AddressLine = "123 Test St",
                City = "Test City",
                ZipCode = "12345",
                Country = "Test Country"
            }
        };

        _senderMock.Setup(s => s.Send(It.IsAny<ReadUserQuery>(), default))
            .ReturnsAsync(userDto);

        // Act
        IActionResult result = await _controller.ReadUser(userId);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userDto, okResult.Value);

        _senderMock.Verify(s => s.Send(It.Is<ReadUserQuery>(q => q.Id == userId), default), Times.Once);
    }
}
