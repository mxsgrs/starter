namespace Network.Infrastructure.IntegrationTests.Facts.SecurityNoteRepositoryTestCases;

[Collection("Database")]
public class RemoveTests(SharedFixture fixture) : IDisposable
{
    [Fact]
    public async Task Remove_ShouldRemoveSecurityNote_WhenCalled()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        SecurityNote note = SecurityNote.Create(user.Id, "Suspicious activity");
        await dbContext.SecurityNotes.AddAsync(note);
        await dbContext.SaveChangesAsync();

        SecurityNoteRepository repository = new(dbContext);

        // Act
        repository.Remove(note);
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
