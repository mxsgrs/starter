namespace Network.Infrastructure.UnitTests.Facts.UserRepositoryTestCases;

public class ReadTrackedUserTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task ReadTrackedUser_ById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.ReadTrackedUser(user.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
    }

    [Fact]
    public async Task ReadTrackedUser_ById_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.ReadTrackedUser(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task ReadTrackedUser_ByEmailAndPassword_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.ReadUserByCredentials("test@example.com", "hashedPassword");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task ReadTrackedUser_ByEmailAndPassword_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.ReadUserByCredentials("nonexistent@example.com", "wrongPassword");

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors[0].Message);
    }
}
