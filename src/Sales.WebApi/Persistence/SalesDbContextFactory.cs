using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sales.WebApi.Persistence;

public class SalesDbContextFactory : IDesignTimeDbContextFactory<SalesDbContext>
{
    /// <summary>
    /// Create a SalesDbContext for EF Core design-time tools (migrations)
    /// </summary>
    public SalesDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<SalesDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=SalesDb;User=sa;Password=B1q22MPXUgosXiqZ;TrustServerCertificate=yes");
        return new SalesDbContext(optionsBuilder.Options);
    }
}
