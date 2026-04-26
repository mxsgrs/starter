using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Network.Domain.Aggregates.UserAggregate;

namespace Network.Infrastructure.Persistance.Configurations;

public class SecurityNoteConfiguration : IEntityTypeConfiguration<SecurityNote>
{
    /// <summary>
    /// Configure the SecurityNote entity mapping with a one-to-one relationship to User
    /// </summary>
    public void Configure(EntityTypeBuilder<SecurityNote> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Reason)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(e => e.UserId).IsUnique();

        builder.HasOne<User>()
            .WithOne(u => u.SecurityNote)
            .HasForeignKey<SecurityNote>(n => n.UserId);
    }
}
