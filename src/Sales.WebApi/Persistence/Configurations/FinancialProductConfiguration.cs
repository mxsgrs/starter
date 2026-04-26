using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sales.WebApi.Persistence.Configurations;

public class FinancialProductConfiguration : IEntityTypeConfiguration<FinancialProduct>
{
    /// <summary>
    /// Configure the FinancialProduct entity mapping
    /// </summary>
    public void Configure(EntityTypeBuilder<FinancialProduct> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1024);
        builder.Property(e => e.Price).HasPrecision(18, 4);
    }
}
