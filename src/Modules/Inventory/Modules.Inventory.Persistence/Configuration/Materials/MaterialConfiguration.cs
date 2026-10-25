using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Persistence.Configuration;

public sealed class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials", table =>
        {
            table.HasCheckConstraint("CK_Materials_PurchaseConversionFactor", "[PurchaseConversionFactor] > 0");
            table.HasCheckConstraint("CK_Materials_MinimumStock", "[MinimumStock] >= 0");
        });
        builder.HasKey(material => material.Id);
        builder.Property(material => material.Name).HasMaxLength(200).IsRequired();
        builder.Property(material => material.Code).HasMaxLength(50).IsRequired();
        builder.Property(material => material.Brand).HasMaxLength(100);
        builder.Property(material => material.PurchaseConversionFactor).HasPrecision(18, 6);
        builder.Property(material => material.MinimumStock).HasPrecision(18, 3);
        builder.Property(material => material.Status).HasColumnName("IsActive").IsRequired();
        builder.Ignore(material => material.IsActive);
        builder.HasIndex(material => material.Code).IsUnique();

        builder.HasOne(material => material.BaseUnit)
            .WithMany(unit => unit.BaseUnitMaterials)
            .HasForeignKey(material => material.BaseUnitId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder.HasOne(material => material.PurchaseUnit)
            .WithMany(unit => unit.PurchaseUnitMaterials)
            .HasForeignKey(material => material.PurchaseUnitId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
