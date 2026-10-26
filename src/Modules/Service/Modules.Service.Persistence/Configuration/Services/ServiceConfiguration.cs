using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Service.Domain.Entities;

namespace Modules.Service.Persistence.Configuration.Services;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Domain.Entities.Service>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Service> builder)
    {
        builder.ToTable("Services", table =>
            table.HasCheckConstraint("CK_Services_DurationMinutes", "[DurationMinutes] > 0"));
        builder.HasKey(service => service.Id);
        builder.Property(service => service.Code).HasMaxLength(50).IsRequired();
        builder.Property(service => service.Name).HasMaxLength(200).IsRequired();
        builder.Property(service => service.Description).HasMaxLength(1000);
        builder.Property(service => service.DurationMinutes).IsRequired();
        builder.Property(service => service.Status).HasColumnName("IsActive").IsRequired();
        builder.Ignore(service => service.IsActive);
        builder.HasAlternateKey(service => new { service.SalonId, service.Id });
        builder.HasIndex(service => new { service.SalonId, service.Code }).IsUnique();
        builder.HasIndex(service => new { service.SalonId, service.CategoryId });

        builder.HasOne(service => service.Category)
            .WithMany(category => category.Services)
            .HasForeignKey(service => new { service.SalonId, service.CategoryId })
            .HasPrincipalKey(category => new { category.SalonId, category.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
