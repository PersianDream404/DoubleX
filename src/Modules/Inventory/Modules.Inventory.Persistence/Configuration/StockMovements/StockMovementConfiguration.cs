using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Persistence.Configuration;

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements", table =>
            table.HasCheckConstraint("CK_StockMovements_Quantity", "[Quantity] > 0"));
        builder.HasKey(movement => movement.Id);
        builder.Property(movement => movement.Type).HasConversion<int>().IsRequired();
        builder.Property(movement => movement.Quantity).HasPrecision(18, 3).IsRequired();
        builder.Property(movement => movement.PurchaseQuantity).HasPrecision(18, 6);
        builder.Property(movement => movement.PurchaseUnitPrice).HasPrecision(18, 2);
        builder.Property(movement => movement.UnitCost).HasPrecision(38, 8);
        builder.Property(movement => movement.TotalCost).HasPrecision(38, 2);
        builder.Property(movement => movement.OccurredAt).IsRequired();
        builder.Property(movement => movement.ReferenceType).HasMaxLength(50);
        builder.Property(movement => movement.Description).HasMaxLength(1000);
        builder.Property(movement => movement.IdempotencyKey).HasMaxLength(100);
        builder.HasIndex(movement => new { movement.MaterialId, movement.OccurredAt });
        builder.HasIndex(movement => movement.IdempotencyKey)
            .IsUnique()
            .HasFilter("[IdempotencyKey] IS NOT NULL");
        builder.HasOne(movement => movement.Material)
            .WithMany(material => material.StockMovements)
            .HasForeignKey(movement => movement.MaterialId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
