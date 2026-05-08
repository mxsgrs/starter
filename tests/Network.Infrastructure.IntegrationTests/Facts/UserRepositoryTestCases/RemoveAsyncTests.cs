namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class RemoveAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task RemoveAsync_ShouldRemoveUserFromDatabase()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        // Act
        Result result = await repository.RemoveAsync(user.Id);

        // Assert
        Assert.True(result.IsSuccess);
        User? deleted = await dbContext.Users.FindAsync(user.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result result = await repository.RemoveAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailed);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
