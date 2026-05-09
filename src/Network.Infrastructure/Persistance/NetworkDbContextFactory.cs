using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Network.Infrastructure.Persistance;

public class NetworkDbContextFactory : IDesignTimeDbContextFactory<NetworkDbContext>
{
    public NetworkDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<NetworkDbContext> optionBuilder = new();

        // docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=B1q22MPXUgosXiqZ" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
        string connectionString = "Data Source=localhost,1433;Initial Catalog=Starter;User=sa;Password=B1q22MPXUgosXiqZ;TrustServerCertificate=yes";
        optionBuilder.UseSqlServer(connectionString);

        // Context used during the creation of migrations
        return new NetworkDbContext(optionBuilder.Options);
    }
}
