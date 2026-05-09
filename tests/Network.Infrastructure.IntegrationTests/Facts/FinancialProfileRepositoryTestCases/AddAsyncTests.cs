namespace Network.Infrastructure.IntegrationTests.Facts.FinancialProfileRepositoryTestCases;

[Collection("Database")]
public class AddAsyncTests(SharedFixture fixture) : IDisposable
{
    [Fact]
    public async Task AddAsync_ShouldStageFinancialProfile_WhenCalled()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        FinancialProfileRepository repository = new(new Mock<ILogger<FinancialProfileRepository>>().Object, dbContext);
        FinancialProfile profile = new FinancialProfileBuilder()
            .WithUserId(user.Id)
            .Build();

        // Act
        await repository.AddAsync(profile);
        await dbContext.SaveChangesAsync();

        // Assert
        FinancialProfile? stored = await dbContext.FinancialProfiles.FindAsync(profile.Id);
        Assert.NotNull(stored);
        Assert.Equal(user.Id, stored.UserId);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.FinancialProfiles.ExecuteDelete();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
