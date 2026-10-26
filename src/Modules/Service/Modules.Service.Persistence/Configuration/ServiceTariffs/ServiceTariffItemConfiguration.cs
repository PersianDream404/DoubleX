using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Service.Domain.Entities;

namespace Modules.Service.Persistence.Configuration.ServiceTariffs;

public sealed class ServiceTariffItemConfiguration : IEntityTypeConfiguration<ServiceTariffItem>
{
    public void Configure(EntityTypeBuilder<ServiceTariffItem> builder)
    {
        builder.ToTable("ServiceTariffItems", table => table.HasCheckConstraint(
            "CK_ServiceTariffItems_Amount",
            "[Amount] >= 0"));
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Amount).HasPrecision(18, 2).IsRequired();
        builder.HasIndex(item => new { item.TariffId, item.ServiceId }).IsUnique();
        builder.HasOne(item => item.Tariff)
            .WithMany(tariff => tariff.Items)
            .HasForeignKey(item => new { item.SalonId, item.TariffId })
            .HasPrincipalKey(tariff => new { tariff.SalonId, tariff.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder.HasOne(item => item.Service)
            .WithMany(service => service.TariffItems)
            .HasForeignKey(item => new { item.SalonId, item.ServiceId })
            .HasPrincipalKey(service => new { service.SalonId, service.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
