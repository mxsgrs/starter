using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace Network.Infrastructure.IntegrationTests.Fixtures;

[CollectionDefinition("Database")]
public sealed class DatabaseCollection : ICollectionFixture<SharedFixture> { }

public class SharedFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .Build();

    private string _connectionString = null!;

    public IAppContextAccessor AppContextAccessor { get; } = new MockContextAccessor();

    /// <summary>
    /// Starts the SQL Server container and applies all EF Core migrations once for the collection.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _connectionString = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = "testdb"
        }.ConnectionString;

        using UserDbContext context = CreateDatabaseContext();
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Returns a new DbContext instance per call so each test starts with a clean identity cache.
    /// </summary>
    public UserDbContext CreateDatabaseContext()
    {
        DbContextOptions<UserDbContext> options = new DbContextOptionsBuilder<UserDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new UserDbContext(options);
    }

    /// <summary>
    /// Stops and disposes the SQL Server container after all tests in the collection have run.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private sealed class MockContextAccessor : IAppContextAccessor
    {
        public UserClaims UserClaims { get; set; } = new()
        {
            UserId = Guid.NewGuid()
        };
    }
}
