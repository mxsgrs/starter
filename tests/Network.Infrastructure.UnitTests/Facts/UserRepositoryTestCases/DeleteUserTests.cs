namespace Network.Infrastructure.UnitTests.Facts.UserRepositoryTestCases;

public class DeleteUserTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task DeleteUser_ShouldRemoveUserFromDatabase()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        // Act
        Result result = await repository.DeleteUser(user.Id);

        // Assert
        Assert.True(result.IsSuccess);
        User? deleted = await dbContext.Users.FindAsync(user.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result result = await repository.DeleteUser(Guid.NewGuid());

        // Assert
        Assert.True(result.IsFailed);
    }
}
