namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class RemoveTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task Remove_ShouldRemoveUserFromDatabase()
    {
        // Arrange
        NetworkDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        // Act
        Result result = await repository.Remove(user.Id);

        // Assert
        Assert.True(result.IsSuccess);
        User? deleted = await dbContext.Users.FindAsync(user.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Remove_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        NetworkDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result result = await repository.Remove(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailed);
    }

    public void Dispose()
    {
        using NetworkDbContext context = fixture.CreateDatabaseContext();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
