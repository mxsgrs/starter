using Network.Application.FinancialProfiles.UseCases;

namespace Network.Application.UnitTests.Facts.FinancialProfiles;

public class UpdateAssetCommandHandlerTests
{
    private readonly Mock<IFinancialProfileRepository> _mockRepository;
    private readonly UpdateAssetCommandHandler _handler;

    public UpdateAssetCommandHandlerTests()
    {
        _mockRepository = new Mock<IFinancialProfileRepository>();
        _handler = new UpdateAssetCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateAsset_WhenProfileAndAssetExist()
    {
        // Arrange
        FinancialProfile profile = new FinancialProfileBuilder()
            .WithAsset("Old Name", AssetType.Stock, 1000m, 0.3m)
            .Build();
        Guid assetId = profile.Assets[0].Id;
        UpdateAssetCommand command = new(profile.UserId, assetId, "New Name", AssetType.Bond, 2000m, 0.5m);

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
        UpdateAssetCommand command = new(userId, Guid.NewGuid(), "Name", AssetType.Stock, 1000m, 0.3m);

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
    public async Task HandleAsync_ShouldReturnFail_WhenAssetNotFound()
    {
        // Arrange
        FinancialProfile profile = new FinancialProfileBuilder().Build();
        UpdateAssetCommand command = new(profile.UserId, Guid.NewGuid(), "Name", AssetType.Stock, 1000m, 0.3m);

        _mockRepository.Setup(r => r.FindByUserId(profile.UserId)).ReturnsAsync(profile);

        // Act
        Result result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailed);

        _mockRepository.Verify(r => r.FindByUserId(profile.UserId), Times.Once);
        _mockRepository.Verify(r => r.Save(), Times.Never);
    }
}
