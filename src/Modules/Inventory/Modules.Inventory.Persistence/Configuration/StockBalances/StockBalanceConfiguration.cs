using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Persistence.Configuration;

public sealed class StockBalanceConfiguration : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.ToTable("StockBalances", table =>
            table.HasCheckConstraint("CK_StockBalances_Quantity", "[Quantity] >= 0"));
        builder.HasKey(balance => balance.Id);
        builder.Property(balance => balance.Quantity).HasPrecision(18, 3).IsRequired();
        builder.Property(balance => balance.RowVersion).IsRowVersion();
        builder.HasIndex(balance => balance.MaterialId).IsUnique();
        builder.HasOne(balance => balance.Material)
            .WithOne(material => material.StockBalance)
            .HasForeignKey<StockBalance>(balance => balance.MaterialId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
