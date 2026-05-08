namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class FindByIdAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task FindByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.FindByIdAsync(user.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.FindByIdAsync(Guid.NewGuid());

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
