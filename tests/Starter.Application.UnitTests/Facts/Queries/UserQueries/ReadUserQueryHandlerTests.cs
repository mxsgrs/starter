using AutoMapper;
using Starter.Application.Dtos;
using Starter.Application.Queries.UserQueries;

namespace Starter.Application.UnitTests.Facts.Queries.UserQueries;

public class ReadUserQueryHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ReadUserQueryHandler _handler;

    public ReadUserQueryHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new ReadUserQueryHandler(_mockMapper.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "test@example.com",
            "hashedPassword",
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("123 Main St", "Metropolis", "12345", "USA")
        );

        var userDto = new UserDto
        {
            Id = userId,
            EmailAddress = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
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

        _mockUserRepository.Setup(repo => repo.ReadUser(userId)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        var query = new ReadUserQuery { Id = userId };

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(userDto.EmailAddress, result.EmailAddress);
        Assert.Equal(userDto.FirstName, result.FirstName);

        _mockUserRepository.Verify(repo => repo.ReadUser(userId), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenMappingFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "test@example.com",
            "hashedPassword",
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("123 Main St", "Metropolis", "12345", "USA")
        );

        _mockUserRepository.Setup(repo => repo.ReadUser(userId)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Throws<AutoMapperMappingException>();

        var query = new ReadUserQuery { Id = userId };

        // Act & Assert
        await Assert.ThrowsAsync<AutoMapperMappingException>(() => _handler.Handle(query, default));

        _mockUserRepository.Verify(repo => repo.ReadUser(userId), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }
}
