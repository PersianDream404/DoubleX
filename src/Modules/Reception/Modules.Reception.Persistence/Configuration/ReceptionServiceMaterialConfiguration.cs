using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Persistence.Configuration;

public sealed class ReceptionServiceMaterialConfiguration
    : IEntityTypeConfiguration<ReceptionServiceMaterial>
{
    public void Configure(EntityTypeBuilder<ReceptionServiceMaterial> builder)
    {
        builder.ToTable("ReceptionServiceMaterials");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Quantity).HasPrecision(18, 3);
        builder.Property(item => item.UnitCost).HasPrecision(38, 8);
        builder.Property(item => item.TotalCost).HasPrecision(18, 2);
        builder.Property(item => item.IdempotencyKey).HasMaxLength(150).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(1000);
        builder.HasIndex(item => new { item.ReceptionServiceId, item.IdempotencyKey }).IsUnique();
        builder.HasIndex(item => item.MaterialId);
    }
}
