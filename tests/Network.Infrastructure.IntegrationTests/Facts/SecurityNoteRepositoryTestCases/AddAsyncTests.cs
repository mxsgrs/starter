namespace Network.Infrastructure.IntegrationTests.Facts.SecurityNoteRepositoryTestCases;

[Collection("Database")]
public class AddAsyncTests(SharedFixture fixture) : IDisposable
{
    [Fact]
    public async Task AddAsync_ShouldStageSecurityNote_WhenCalled()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        SecurityNoteRepository repository = new(dbContext);
        SecurityNote note = SecurityNote.Create(user.Id, "Suspicious activity");

        // Act
        await repository.AddAsync(note, CancellationToken.None);
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
