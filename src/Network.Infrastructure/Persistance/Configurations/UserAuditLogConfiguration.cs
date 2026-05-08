using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Network.Domain.Aggregates.UserAuditLogAggregate;

namespace Network.Infrastructure.Persistance.Configurations;

public class UserAuditLogConfiguration : IEntityTypeConfiguration<UserAuditLog>
{
    /// <summary>
    /// Configure the UserAuditLog entity mapping
    /// </summary>
    public void Configure(EntityTypeBuilder<UserAuditLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(e => e.UserId);
    }
}
