using AutoMapper;
using UserService.Application.Dtos.UserDtos;
using UserService.Application.Queries.UserQueries;

namespace UserService.Application.UnitTests.Facts.Queries.UserQueries;

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
        var user = new UserBuilder().WithId(userId).Build();
        var userDto = new UserDtoBuilder().WithId(userId).Build();

        _mockUserRepository.Setup(repo => repo.ReadUser(userId)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        Result<UserDto> result = await _handler.HandleAsync(userId, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal(userDto.EmailAddress, result.Value.EmailAddress);
        Assert.Equal(userDto.FirstName, result.Value.FirstName);

        _mockUserRepository.Verify(repo => repo.ReadUser(userId), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenMappingFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserBuilder().WithId(userId).Build();

        _mockUserRepository.Setup(repo => repo.ReadUser(userId)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Throws<AutoMapperMappingException>();

        // Act & Assert
        await Assert.ThrowsAsync<AutoMapperMappingException>(() => _handler.HandleAsync(userId, default));

        _mockUserRepository.Verify(repo => repo.ReadUser(userId), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }
}
