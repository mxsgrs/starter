using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Sales.WebApi.Persistence;

public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
    {
        string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "Development") Database.Migrate();

        if (!FinancialProducts.Any())
        {
            FinancialProducts.AddRange(
                FinancialProduct.Create("Savings Account", "Standard savings account with competitive interest rate.", 0m),
                FinancialProduct.Create("Term Deposit", "Fixed-term deposit with guaranteed return.", 1000m),
                FinancialProduct.Create("Investment Fund", "Diversified investment fund for long-term growth.", 5000m)
            );
            SaveChanges();
        }
    }

    public virtual DbSet<FinancialProduct> FinancialProducts { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
