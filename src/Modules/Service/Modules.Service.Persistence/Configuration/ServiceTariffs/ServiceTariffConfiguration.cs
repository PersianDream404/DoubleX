using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Configuration.ServiceTariffs;

public sealed class ServiceTariffConfiguration : IEntityTypeConfiguration<ServiceTariff>
{
    public void Configure(EntityTypeBuilder<ServiceTariff> builder)
    {
        builder.ToTable("ServiceTariffs");
        builder.HasKey(tariff => tariff.Id);
        builder.Property(tariff => tariff.Name).HasMaxLength(200).IsRequired();
        builder.ToTable("ServiceTariffs", table => table.HasCheckConstraint(
            "CK_ServiceTariffs_Validity",
            "[ValidTo] IS NULL OR [ValidTo] > [ValidFrom]"));
        builder.Property(tariff => tariff.ValidFrom).IsRequired();
        builder.HasAlternateKey(tariff => new { tariff.SalonId, tariff.Id });
        builder.HasIndex(tariff => new { tariff.SalonId, tariff.Name }).IsUnique();
        builder.HasOne<ServiceSalonReference>()
            .WithMany()
            .HasForeignKey(tariff => tariff.SalonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
