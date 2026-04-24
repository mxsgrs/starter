namespace UserService.Infrastructure.UnitTests.Facts.UserRepositoryTestCases;

public class CreateUserTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);
        User user = new UserBuilder().Build();

        // Act
        _ = await repository.CreateUser(user);

        // Assert
        User? storedUser = await dbContext.Users.FindAsync(user.Id);
        Assert.NotNull(storedUser);
        Assert.Equal("test@example.com", storedUser.EmailAddress);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFail_WhenUserAlreadyExists()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User duplicate = new UserBuilder().WithId(Guid.NewGuid()).Build();

        // Act
        Result<User> result = await repository.CreateUser(duplicate);

        // Assert
        Assert.True(result.IsFailed);
    }
}
