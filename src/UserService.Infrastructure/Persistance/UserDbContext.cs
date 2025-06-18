using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserService.Domain.Aggregates.UserAggregate;

namespace UserService.Infrastructure.Persistance;

public partial class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
        string? aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (aspNetCoreEnvironment == "Development")
        {
            Database.Migrate();
        }
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
