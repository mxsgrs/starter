namespace Network.Infrastructure.IntegrationTests.Facts.UserRepositoryTestCases;

[Collection("Database")]
public class RemoveSecurityNoteTests(SharedFixture fixture) : IDisposable
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task RemoveSecurityNote_ShouldRemoveSecurityNote_WhenCalled()
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
        repository.RemoveSecurityNote(note);
        await dbContext.SaveChangesAsync();

        // Assert
        SecurityNote? deleted = await dbContext.SecurityNotes.FindAsync(note.Id);
        Assert.Null(deleted);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.SecurityNotes.ExecuteDelete();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
