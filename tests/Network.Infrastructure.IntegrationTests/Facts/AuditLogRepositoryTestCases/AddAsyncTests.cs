namespace Network.Infrastructure.IntegrationTests.Facts.AuditLogRepositoryTestCases;

[Collection("Database")]
public class AddAsyncTests(SharedFixture fixture) : IDisposable
{
    [Fact]
    public async Task AddAsync_ShouldStageAuditLog_WhenCalled()
    {
        // Arrange
        UserDbContext dbContext = fixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        AuditLogRepository repository = new(dbContext);
        AuditLog auditLog = new AuditLogBuilder()
            .WithUserId(user.Id)
            .WithEventType(AuditLogEventType.UserCreated)
            .Build();

        // Act
        await repository.AddAsync(auditLog);
        await dbContext.SaveChangesAsync();

        // Assert
        AuditLog? stored = await dbContext.AuditLogs.FindAsync(auditLog.Id);
        Assert.NotNull(stored);
        Assert.Equal(user.Id, stored.UserId);
        Assert.Equal(AuditLogEventType.UserCreated, stored.EventType);
    }

    public void Dispose()
    {
        using UserDbContext context = fixture.CreateDatabaseContext();
        context.AuditLogs.ExecuteDelete();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
}
