namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class AddSecurityNoteAsyncTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task AddSecurityNoteAsync_ShouldStageSecurityNote_WhenCalled()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);
        SecurityNote note = SecurityNote.Create(user.Id, "Suspicious activity").Value;

        // Act
        await repository.AddSecurityNoteAsync(note);
        await dbContext.SaveChangesAsync();

        // Assert
        SecurityNote? stored = await dbContext.SecurityNotes.FindAsync(note.Id);
        Assert.NotNull(stored);
        Assert.Equal(user.Id, stored.UserId);
        Assert.Equal("Suspicious activity", stored.Reason);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.SecurityNotes.ExecuteDelete();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
