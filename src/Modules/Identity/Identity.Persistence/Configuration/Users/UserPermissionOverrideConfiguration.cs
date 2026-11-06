using Identity.Domain.Entities;
using Identity.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configuration.Users;

public sealed class UserPermissionOverrideConfiguration
    : IEntityTypeConfiguration<UserPermissionOverride>
{
    public void Configure(EntityTypeBuilder<UserPermissionOverride> builder)
    {
        builder.ToTable("UserPermissionOverrides", table =>
            table.HasCheckConstraint(
                "CK_UserPermissionOverrides_Effect",
                "[Effect] IN (1, 2)"));

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Permission).HasMaxLength(150).IsRequired();
        builder.Property(item => item.Effect).HasConversion<int>().IsRequired();
        builder.Property(item => item.IsActive).IsRequired();
        builder.HasIndex(item => new { item.UserId, item.SalonId, item.Permission }).IsUnique();
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(item => item.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
