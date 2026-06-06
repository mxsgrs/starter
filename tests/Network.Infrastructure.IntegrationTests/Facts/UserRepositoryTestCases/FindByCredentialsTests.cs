namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class FindByCredentialsTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task FindByCredentials_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        NetworkDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.FindByCredentials("test@example.com", "hashedPassword");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task FindByCredentials_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        NetworkDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.FindByCredentials("nonexistent@example.com", "wrongPassword");

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors[0].Message);
    }

    public void Dispose()
    {
        using NetworkDbContext context = fixture.CreateDatabaseContext();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
