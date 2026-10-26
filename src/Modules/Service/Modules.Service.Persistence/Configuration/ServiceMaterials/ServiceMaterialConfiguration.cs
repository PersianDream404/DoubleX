using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Configuration.ServiceMaterials;

public sealed class ServiceMaterialConfiguration : IEntityTypeConfiguration<ServiceMaterial>
{
    public void Configure(EntityTypeBuilder<ServiceMaterial> builder)
    {
        builder.ToTable("ServiceMaterials", table =>
            table.HasCheckConstraint(
                "CK_ServiceMaterials_QuantityPerService",
                "[QuantityPerService] > 0"));
        builder.HasKey(material => material.Id);
        builder.Property(material => material.QuantityPerService)
            .HasPrecision(18, 3)
            .IsRequired();
        builder.Property(material => material.Description).HasMaxLength(1000);
        builder.HasIndex(material => new { material.ServiceId, material.MaterialId })
            .IsUnique()
            .HasFilter("[Deleted] = 0");
        builder.HasIndex(material => material.SalonId);
        builder.HasOne(material => material.Service)
            .WithMany(service => service.Materials)
            .HasForeignKey(material => new { material.SalonId, material.ServiceId })
            .HasPrincipalKey(service => new { service.SalonId, service.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder.HasOne<ServiceInventoryMaterialReference>()
            .WithMany()
            .HasForeignKey(material => material.MaterialId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
