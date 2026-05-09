using Network.Application.FinancialProfiles.UseCases;

namespace Network.Application.UnitTests.Facts.FinancialProfiles;

public class ReadFinancialProfileQueryHandlerTests
{
    private readonly Mock<IFinancialProfileRepository> _mockRepository;
    private readonly ReadFinancialProfileQueryHandler _handler;

    public ReadFinancialProfileQueryHandlerTests()
    {
        FinancialProfileMapping.Register(TypeAdapterConfig.GlobalSettings);
        _mockRepository = new Mock<IFinancialProfileRepository>();
        _handler = new ReadFinancialProfileQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFinancialProfileDto_WhenProfileExists()
    {
        // Arrange
        FinancialProfile profile = new FinancialProfileBuilder().Build();

        _mockRepository.Setup(r => r.FindByUserIdAsync(profile.UserId)).ReturnsAsync(profile);

        // Act
        Result<FinancialProfileDto> result = await _handler.HandleAsync(profile.UserId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(profile.Id, result.Value.Id);
        Assert.Equal(profile.UserId, result.Value.UserId);
        Assert.Equal(profile.RiskScore, result.Value.RiskScore);

        _mockRepository.Verify(r => r.FindByUserIdAsync(profile.UserId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenProfileNotFound()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        _mockRepository.Setup(r => r.FindByUserIdAsync(userId))
            .ReturnsAsync(Result.Fail<FinancialProfile>("Financial profile not found"));

        // Act
        Result<FinancialProfileDto> result = await _handler.HandleAsync(userId);

        // Assert
        Assert.True(result.IsFailed);

        _mockRepository.Verify(r => r.FindByUserIdAsync(userId), Times.Once);
    }
}
