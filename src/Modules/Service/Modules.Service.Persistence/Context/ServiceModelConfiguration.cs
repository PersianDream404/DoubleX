using Microsoft.EntityFrameworkCore;

namespace Modules.Service.Persistence.Context;

internal static class ServiceModelConfiguration
{
    public static void ConfigureReferences(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceSalonReference>(builder =>
        {
            builder.ToTable("Salons", "Salon", table => table.ExcludeFromMigrations());
            builder.HasKey(salon => salon.Id);
        });
        modelBuilder.Entity<ServiceInventoryMaterialReference>(builder =>
        {
            builder.ToTable("Materials", "Inventory", table => table.ExcludeFromMigrations());
            builder.HasKey(material => material.Id);
            builder.Property(material => material.IsActive).HasColumnName("IsActive");
        });
        modelBuilder.Entity<ServiceInventoryUnitReference>(builder =>
        {
            builder.ToTable("Units", "Inventory", table => table.ExcludeFromMigrations());
            builder.HasKey(unit => unit.Id);
        });
        modelBuilder.Entity<ServicePersonnelReference>(builder =>
        {
            builder.ToTable("Personnels", "Salon", table => table.ExcludeFromMigrations());
            builder.HasKey(personnel => personnel.Id);
        });
    }
}

internal sealed class ServiceSalonReference
{
    public long Id { get; set; }
}

internal sealed class ServiceInventoryMaterialReference
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long BaseUnitId { get; set; }
    public bool IsActive { get; set; }
}

internal sealed class ServiceInventoryUnitReference
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Symbol { get; set; } = null!;
}

internal sealed class ServicePersonnelReference
{
    public long Id { get; set; }
}
