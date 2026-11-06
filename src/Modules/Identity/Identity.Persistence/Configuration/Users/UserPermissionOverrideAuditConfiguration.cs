using Identity.Domain.Entities;
using Identity.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configuration.Users;

public sealed class UserPermissionOverrideAuditConfiguration
    : IEntityTypeConfiguration<UserPermissionOverrideAudit>
{
    public void Configure(EntityTypeBuilder<UserPermissionOverrideAudit> builder)
    {
        builder.ToTable("UserPermissionOverrideAudits", table =>
            table.HasCheckConstraint(
                "CK_UserPermissionOverrideAudits_Effect",
                "[Effect] IS NULL OR [Effect] IN (1, 2)"));

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Permission).HasMaxLength(150).IsRequired();
        builder.Property(item => item.Effect).HasConversion<int?>();
        builder.Property(item => item.Action).HasMaxLength(20).IsRequired();
        builder.HasIndex(item => new { item.UserId, item.SalonId, item.CreatedAt });
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(item => item.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
