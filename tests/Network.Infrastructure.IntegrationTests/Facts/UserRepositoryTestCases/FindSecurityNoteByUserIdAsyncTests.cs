namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class FindSecurityNoteByUserIdAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task FindSecurityNoteByUserIdAsync_ShouldReturnSecurityNote_WhenExists()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        SecurityNote note = SecurityNote.Create(user.Id, "Suspicious activity").Value;
        await dbContext.SecurityNotes.AddAsync(note);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        SecurityNote? result = await repository.FindSecurityNoteByUserIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(note.Id, result.Id);
    }

    [Fact]
    public async Task FindSecurityNoteByUserIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        SecurityNote? result = await repository.FindSecurityNoteByUserIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.SecurityNotes.ExecuteDelete();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
