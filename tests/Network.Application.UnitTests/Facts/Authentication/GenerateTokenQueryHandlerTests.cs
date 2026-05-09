using Network.Application.Authentication.Dtos;
using Network.Application.Authentication.UseCases;
using Network.Application.UnitTests.Fixtures;

namespace Network.Application.UnitTests.Facts.Authentication;

public class GenerateTokenQueryHandlerTests : IClassFixture<SharedFixture>
{
    private readonly Mock<ILogger<GenerateTokenQueryHandler>> _mockLogger;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GenerateTokenQueryHandler _handler;

    public GenerateTokenQueryHandlerTests(SharedFixture fixture)
    {
        _mockLogger = new Mock<ILogger<GenerateTokenQueryHandler>>();
        _mockUserRepository = new Mock<IUserRepository>();

        _handler = new GenerateTokenQueryHandler(
            _mockLogger.Object,
            fixture.Configuration,
            _mockUserRepository.Object
        );
    }

    [Fact]
    public async Task Handle_InvalidUser_ReturnsFailure()
    {
        // Arrange
        string emailAddress = "invalid@example.com";
        string hashedPassword = "wrongPassword";

        _mockUserRepository
            .Setup(repo => repo.FindByCredentialsAsync(emailAddress, hashedPassword))
            .ReturnsAsync(Result.Fail("User not found"));

        GenerateTokenQuery command = new(emailAddress, hashedPassword);

        // Act
        Result<LoginResponseDto> result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ValidUser_ReturnsAccessToken()
    {
        // Arrange
        string emailAddress = "test@example.com";
        string hashedPassword = "hashedPassword123";

        User user = new UserBuilder()
            .WithHashedPassword(hashedPassword)
            .WithFirstName("Test")
            .WithLastName("User")
            .Build();

        _mockUserRepository
            .Setup(repo => repo.FindByCredentialsAsync(emailAddress, hashedPassword))
            .ReturnsAsync(user);

        GenerateTokenQuery command = new(emailAddress, hashedPassword);

        // Act
        Result<LoginResponseDto> result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.AccessToken);
        Assert.NotEmpty(result.Value.AccessToken);
    }
}
