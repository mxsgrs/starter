namespace Network.Infrastructure.IntegrationTests.Facts.FinancialProfileRepositoryTestCases;

[Collection("Database")]
public class FindByUserIdAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<FinancialProfileRepository>> _logger = new();

    [Fact]
    public async Task FindByUserIdAsync_ShouldReturnProfile_WhenProfileExists()
    {
        // Arrange
        NetworkDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        FinancialProfile profile = new FinancialProfileBuilder()
            .WithUserId(user.Id)
            .Build();
        await dbContext.FinancialProfiles.AddAsync(profile);
        await dbContext.SaveChangesAsync();

        FinancialProfileRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<FinancialProfile> result = await repository.FindByUserIdAsync(user.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(profile.Id, result.Value.Id);
        Assert.Equal(user.Id, result.Value.UserId);
    }

    [Fact]
    public async Task FindByUserIdAsync_ShouldReturnFail_WhenProfileNotFound()
    {
        // Arrange
        NetworkDbContext dbContext = fixture.CreateDatabaseContext();
        FinancialProfileRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<FinancialProfile> result = await repository.FindByUserIdAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Financial profile not found", result.Errors[0].Message);
    }

    public void Dispose()
    {
        using NetworkDbContext context = fixture.CreateDatabaseContext();
        context.FinancialProfiles.ExecuteDelete();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
