using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Persistence.Configuration;

public sealed class ReceptionServiceConfiguration : IEntityTypeConfiguration<ReceptionService>
{
    public void Configure(EntityTypeBuilder<ReceptionService> builder)
    {
        builder.ToTable("ReceptionServices");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ServiceName).HasMaxLength(250).IsRequired();
        builder.Property(item => item.Quantity).HasPrecision(18, 3);
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
        builder.Property(item => item.DiscountAmount).HasPrecision(18, 2);
        builder.Property(item => item.FinalAmount).HasPrecision(18, 2);
        builder.HasIndex(item => new { item.ReceptionId, item.ServiceId });
        builder.HasMany(item => item.Materials)
            .WithOne(material => material.ReceptionService)
            .HasForeignKey(material => material.ReceptionServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
