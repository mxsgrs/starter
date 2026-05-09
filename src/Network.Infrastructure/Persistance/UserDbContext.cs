using Microsoft.EntityFrameworkCore;
using Network.Domain.Aggregates.FinancialProfileAggregate;
using Network.Domain.Aggregates.UserAggregate;
using System.Reflection;

namespace Network.Infrastructure.Persistance;

public partial class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "Development") Database.Migrate();
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<FinancialProfile> FinancialProfiles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
