namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class AddAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);
        User user = new UserBuilder().Build();

        // Act
        _ = await repository.AddAsync(user);

        // Assert
        User? storedUser = await dbContext.Users.FindAsync(user.Id);
        Assert.NotNull(storedUser);
        Assert.Equal("test@example.com", storedUser.EmailAddress);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnFail_WhenUserAlreadyExists()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User duplicate = new UserBuilder().Build();

        // Act
        Result<User> result = await repository.AddAsync(duplicate);

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
