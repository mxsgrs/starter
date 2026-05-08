using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.WebApi.Domain;

namespace Sales.WebApi.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    /// <summary>
    /// Configure the Contract entity mapping
    /// </summary>
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.FinancialProduct)
            .WithMany()
            .HasForeignKey(e => e.FinancialProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
