namespace Network.Infrastructure.IntegrationTests.Facts.FinancialProfileRepositoryTestCases;

[Collection("Database")]
public class UpdateAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<FinancialProfileRepository>> _logger = new();

    [Fact]
    public async Task UpdateAsync_ShouldPersistAssetAddition_WhenProfileExists()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        FinancialProfile profile = new FinancialProfileBuilder()
            .WithUserId(user.Id)
            .Build();
        await dbContext.FinancialProfiles.AddAsync(profile);
        await dbContext.SaveChangesAsync();

        FinancialProfileRepository repository = new(_logger.Object, dbContext);
        Result<FinancialProfile> loadResult = await repository.FindByUserIdAsync(user.Id);
        loadResult.Value.AddAsset("Bond A", AssetType.Bond, 3000m, 0.2m);

        // Act
        Result result = await repository.UpdateAsync(loadResult.Value.Id);

        // Assert
        Assert.True(result.IsSuccess);

        UserDbContext verifyContext = fixture.CreateDatabaseContext();
        FinancialProfile? stored = await verifyContext.FinancialProfiles
            .Include(fp => fp.Assets)
            .FirstOrDefaultAsync(fp => fp.Id == profile.Id);
        Assert.NotNull(stored);
        Assert.Single(stored.Assets);
        Assert.Equal("Bond A", stored.Assets[0].Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFail_WhenProfileNotFound()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        FinancialProfileRepository repository = new(_logger.Object, dbContext);

        // Act
        Result result = await repository.UpdateAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Financial profile not found", result.Errors[0].Message);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.FinancialProfiles.ExecuteDelete();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
