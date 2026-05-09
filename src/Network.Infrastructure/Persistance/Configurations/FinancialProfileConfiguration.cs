using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Network.Domain.Aggregates.FinancialProfileAggregate;
using Network.Domain.Aggregates.UserAggregate;

namespace Network.Infrastructure.Persistance.Configurations;

public class FinancialProfileConfiguration : IEntityTypeConfiguration<FinancialProfile>
{
    /// <summary>
    /// Configure the FinancialProfile and owned Asset entity mappings.
    /// </summary>
    public void Configure(EntityTypeBuilder<FinancialProfile> builder)
    {
        builder.HasKey(fp => fp.Id);

        builder.HasIndex(fp => fp.UserId).IsUnique();

        // Cascade delete when the owning User is deleted
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<FinancialProfile>(fp => fp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(fp => fp.RiskScore)
            .HasColumnType("decimal(5,4)")
            .IsRequired();

        // Assets are owned entities — their lifecycle is bound to this aggregate root
        builder.OwnsMany(fp => fp.Assets, assetBuilder =>
        {
            assetBuilder.ToTable("FinancialProfileAssets");

            assetBuilder.WithOwner()
                .HasForeignKey(a => a.FinancialProfileId);

            assetBuilder.HasKey(a => a.Id);

            assetBuilder.Property(a => a.Id).ValueGeneratedNever();

            assetBuilder.UsePropertyAccessMode(PropertyAccessMode.Field);

            assetBuilder.Property(a => a.Name)
                .HasMaxLength(128)
                .IsRequired();

            assetBuilder.Property(a => a.AssetType)
                .HasConversion<string>()
                .HasMaxLength(64)
                .IsRequired();

            assetBuilder.Property(a => a.Value)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            assetBuilder.Property(a => a.RiskFactor)
                .HasColumnType("decimal(5,4)")
                .IsRequired();
        });

        // Tell EF Core to use the _assets backing field for the Assets navigation
        builder.Navigation(fp => fp.Assets)
            .HasField("_assets")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
