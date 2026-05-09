using Network.Application.FinancialProfiles.UseCases;

namespace Network.Application.UnitTests.Facts.FinancialProfiles;

public class DeleteAssetCommandHandlerTests
{
    private readonly Mock<IFinancialProfileRepository> _mockRepository;
    private readonly DeleteAssetCommandHandler _handler;

    public DeleteAssetCommandHandlerTests()
    {
        _mockRepository = new Mock<IFinancialProfileRepository>();
        _handler = new DeleteAssetCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveAsset_WhenProfileAndAssetExist()
    {
        // Arrange
        FinancialProfile profile = new FinancialProfileBuilder()
            .WithAsset("Test Asset", AssetType.Stock, 1000m, 0.3m)
            .Build();
        Guid assetId = profile.Assets[0].Id;
        DeleteAssetCommand command = new(profile.UserId, assetId);

        _mockRepository.Setup(r => r.FindByUserIdAsync(profile.UserId)).ReturnsAsync(profile);
        _mockRepository.Setup(r => r.UpdateAsync(profile.Id)).ReturnsAsync(Result.Ok());

        // Act
        Result result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        _mockRepository.Verify(r => r.FindByUserIdAsync(profile.UserId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(profile.Id), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenProfileNotFound()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        DeleteAssetCommand command = new(userId, Guid.NewGuid());

        _mockRepository.Setup(r => r.FindByUserIdAsync(userId))
            .ReturnsAsync(Result.Fail<FinancialProfile>("Financial profile not found"));

        // Act
        Result result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailed);

        _mockRepository.Verify(r => r.FindByUserIdAsync(userId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenAssetNotFound()
    {
        // Arrange
        FinancialProfile profile = new FinancialProfileBuilder().Build();
        DeleteAssetCommand command = new(profile.UserId, Guid.NewGuid());

        _mockRepository.Setup(r => r.FindByUserIdAsync(profile.UserId)).ReturnsAsync(profile);

        // Act
        Result result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailed);

        _mockRepository.Verify(r => r.FindByUserIdAsync(profile.UserId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Guid>()), Times.Never);
    }
}
