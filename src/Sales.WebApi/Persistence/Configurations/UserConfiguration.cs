using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sales.WebApi.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configure the User entity mapping
    /// </summary>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
    }
}
