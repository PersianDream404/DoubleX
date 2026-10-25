using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Persistence.Configuration;

public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");
        builder.HasKey(unit => unit.Id);
        builder.Property(unit => unit.Name).HasMaxLength(100).IsRequired();
        builder.Property(unit => unit.Symbol).HasMaxLength(20).IsRequired();
        builder.Property(unit => unit.UnitType).HasConversion<int>().IsRequired();
        builder.Property(unit => unit.Status).HasColumnName("IsActive").IsRequired();
        builder.Ignore(unit => unit.IsActive);
        builder.HasIndex(unit => unit.Name).IsUnique();
        builder.HasIndex(unit => unit.Symbol).IsUnique();
    }
}
