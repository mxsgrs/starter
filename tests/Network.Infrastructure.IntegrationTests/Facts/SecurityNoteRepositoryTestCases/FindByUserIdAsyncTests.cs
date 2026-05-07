namespace Network.Infrastructure.IntegrationTests.Facts.SecurityNoteRepositoryTestCases;

[Collection("Database")]
public class FindByUserIdAsyncTests(SharedFixture fixture) : IDisposable
{
    [Fact]
    public async Task FindByUserIdAsync_ShouldReturnSecurityNote_WhenExists()
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
        SecurityNote? result = await repository.FindByUserIdAsync(user.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(note.Id, result.Id);
    }

    [Fact]
    public async Task FindByUserIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        SecurityNoteRepository repository = new(dbContext);

        // Act
        SecurityNote? result = await repository.FindByUserIdAsync(Guid.NewGuid(), CancellationToken.None);

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
