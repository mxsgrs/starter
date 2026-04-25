using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserService.Domain.Aggregates.UserAggregate;

namespace UserService.Infrastructure.Persistance;

public partial class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "Development") Database.Migrate();
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserAuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
