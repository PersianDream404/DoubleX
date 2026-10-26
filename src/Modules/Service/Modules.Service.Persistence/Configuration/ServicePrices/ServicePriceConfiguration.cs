using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Service.Domain.Entities;

namespace Modules.Service.Persistence.Configuration.ServicePrices;

public sealed class ServicePriceConfiguration : IEntityTypeConfiguration<ServicePrice>
{
    public void Configure(EntityTypeBuilder<ServicePrice> builder)
    {
        builder.ToTable("ServicePrices", table =>
        {
            table.HasCheckConstraint("CK_ServicePrices_Amount", "[Amount] >= 0");
            table.HasCheckConstraint(
                "CK_ServicePrices_Validity",
                "[ValidTo] IS NULL OR [ValidTo] > [ValidFrom]");
        });
        builder.HasKey(price => price.Id);
        builder.Property(price => price.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(price => price.ValidFrom).IsRequired();
        builder.HasIndex(price => new { price.ServiceId, price.ValidFrom });
        builder.HasOne(price => price.Service)
            .WithMany(service => service.Prices)
            .HasForeignKey(price => new { price.SalonId, price.ServiceId })
            .HasPrincipalKey(service => new { service.SalonId, service.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
