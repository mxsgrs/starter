using Starter.Application.Commands.AuthCommands;
using Starter.Application.UnitTests.Facts.Fixtures;

namespace Starter.Application.UnitTests.Facts.Commands.AuthCommands;

public class CreateTokenCommandHandlerTests : IClassFixture<SharedFixture>
{
    private readonly Mock<ILogger<CreateTokenCommandHandler>> _mockLogger;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateTokenCommandHandler _handler;

    public CreateTokenCommandHandlerTests(SharedFixture fixture)
    {
        _mockLogger = new Mock<ILogger<CreateTokenCommandHandler>>();
        _mockUserRepository = new Mock<IUserRepository>();

        _handler = new CreateTokenCommandHandler(
            _mockLogger.Object,
            fixture.Configuration,
            _mockUserRepository.Object
        );
    }

    [Fact]
    public async Task Handle_InvalidUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var emailAddress = "invalid@example.com";
        var hashedPassword = "wrongPassword";

        _mockUserRepository
            .Setup(repo => repo.ReadUser(emailAddress, hashedPassword))
            .ThrowsAsync(new UnauthorizedException());

        var command = new CreateTokenCommand(emailAddress, hashedPassword);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidUser_ReturnsAccessToken()
    {
        // Arrange
        var emailAddress = "test@example.com";
        var hashedPassword = "hashedPassword123";
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            emailAddress,
            hashedPassword,
            "Test",
            "User",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("Street", "City", "State", "PostalCode", "Country")
        );

        var jwtParameters = new JsonWebTokenParameters
        {
            Key = "test_secret_key_1234567890",
            Issuer = "testIssuer",
            Audience = "testAudience"
        };

        _mockUserRepository
            .Setup(repo => repo.ReadUser(emailAddress, hashedPassword))
            .ReturnsAsync(user);

        var command = new CreateTokenCommand(emailAddress, hashedPassword);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
    }
}

