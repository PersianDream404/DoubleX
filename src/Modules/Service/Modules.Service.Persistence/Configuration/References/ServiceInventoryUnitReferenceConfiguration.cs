using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modules.Service.Persistence.Context;

internal sealed class ServiceInventoryUnitReference
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Symbol { get; set; } = null!;
}

internal sealed class ServiceInventoryUnitReferenceConfiguration
    : IEntityTypeConfiguration<ServiceInventoryUnitReference>
{
    public void Configure(EntityTypeBuilder<ServiceInventoryUnitReference> builder)
    {
        builder.ToTable("Units", "Inventory", table => table.ExcludeFromMigrations());
        builder.HasKey(unit => unit.Id);
    }
}
