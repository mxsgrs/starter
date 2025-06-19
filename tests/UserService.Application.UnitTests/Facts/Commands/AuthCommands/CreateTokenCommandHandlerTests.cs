using UserService.Application.Commands.AuthCommands;
using UserService.Application.Dtos;
using UserService.Application.UnitTests.Facts.Fixtures;

namespace UserService.Application.UnitTests.Facts.Commands.AuthCommands;

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
    public async Task Handle_InvalidUser_ReturnsFailure()
    {
        // Arrange
        string emailAddress = "invalid@example.com";
        string hashedPassword = "wrongPassword";

        _mockUserRepository
            .Setup(repo => repo.ReadUser(emailAddress, hashedPassword))
            .ReturnsAsync(Result.Fail("User not found"));

        CreateTokenCommand command = new(emailAddress, hashedPassword);

        // Act
        Result<LoginResponseDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ValidUser_ReturnsAccessToken()
    {
        // Arrange
        string emailAddress = "test@example.com";
        string hashedPassword = "hashedPassword123";
        Guid userId = Guid.NewGuid();
        User user = new(
            userId,
            emailAddress,
            hashedPassword,
            "Test",
            "User",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address(
                "Street", 
                "City",
                "State",
                "PostalCode",
                "Country"
            )
        );

        JsonWebTokenParameters jwtParameters = new()
        {
            Key = "test_secret_key_1234567890",
            Issuer = "testIssuer",
            Audience = "testAudience"
        };

        _mockUserRepository
            .Setup(repo => repo.ReadUser(emailAddress, hashedPassword))
            .ReturnsAsync(user);

        CreateTokenCommand command = new(emailAddress, hashedPassword);

        // Act
        Result<LoginResponseDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.AccessToken);
        Assert.NotEmpty(result.Value.AccessToken);
    }
}

