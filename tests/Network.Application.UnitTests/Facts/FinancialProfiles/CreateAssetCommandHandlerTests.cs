using Network.Application.FinancialProfiles.UseCases;

namespace Network.Application.UnitTests.Facts.FinancialProfiles;

public class CreateAssetCommandHandlerTests
{
    private readonly Mock<IFinancialProfileRepository> _mockRepository;
    private readonly CreateAssetCommandHandler _handler;

    public CreateAssetCommandHandlerTests()
    {
        _mockRepository = new Mock<IFinancialProfileRepository>();
        _handler = new CreateAssetCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddAsset_WhenProfileExistsAndInputIsValid()
    {
        // Arrange
        FinancialProfile profile = new FinancialProfileBuilder().Build();
        CreateAssetCommand command = new(profile.UserId, "Test Bond", AssetType.Bond, 5000m, 0.2m);

        _mockRepository.Setup(r => r.FindByUserId(profile.UserId)).ReturnsAsync(profile);
        _mockRepository.Setup(r => r.Save()).ReturnsAsync(Result.Ok());

        // Act
        Result result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        _mockRepository.Verify(r => r.FindByUserId(profile.UserId), Times.Once);
        _mockRepository.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenProfileNotFound()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        CreateAssetCommand command = new(userId, "Test Bond", AssetType.Bond, 5000m, 0.2m);

        _mockRepository.Setup(r => r.FindByUserId(userId))
            .ReturnsAsync(Result.Fail<FinancialProfile>("Financial profile not found"));

        // Act
        Result result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailed);

        _mockRepository.Verify(r => r.FindByUserId(userId), Times.Once);
        _mockRepository.Verify(r => r.Save(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUpdateFails()
    {
        // Arrange
        FinancialProfile profile = new FinancialProfileBuilder().Build();
        CreateAssetCommand command = new(profile.UserId, "Test Bond", AssetType.Bond, 5000m, 0.2m);

        _mockRepository.Setup(r => r.FindByUserId(profile.UserId)).ReturnsAsync(profile);
        _mockRepository.Setup(r => r.Save()).ReturnsAsync(Result.Fail("Database error"));

        // Act
        Result result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailed);

        _mockRepository.Verify(r => r.Save(), Times.Once);
    }
}
