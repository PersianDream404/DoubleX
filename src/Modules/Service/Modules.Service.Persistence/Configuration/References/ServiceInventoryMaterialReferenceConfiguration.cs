using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modules.Service.Persistence.Context;

internal sealed class ServiceInventoryMaterialReference
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long BaseUnitId { get; set; }
    public bool IsActive { get; set; }
}

internal sealed class ServiceInventoryMaterialReferenceConfiguration
    : IEntityTypeConfiguration<ServiceInventoryMaterialReference>
{
    public void Configure(EntityTypeBuilder<ServiceInventoryMaterialReference> builder)
    {
        builder.ToTable("Materials", "Inventory", table => table.ExcludeFromMigrations());
        builder.HasKey(material => material.Id);
        builder.Property(material => material.IsActive).HasColumnName("IsActive");
    }
}
