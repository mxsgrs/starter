namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class FindByCredentialsAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task FindByCredentialsAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.FindByCredentialsAsync("test@example.com", "hashedPassword");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task FindByCredentialsAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.FindByCredentialsAsync("nonexistent@example.com", "wrongPassword");

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors[0].Message);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
