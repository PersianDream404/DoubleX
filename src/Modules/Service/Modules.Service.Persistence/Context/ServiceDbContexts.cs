using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Service.Domain.Entities;

namespace Modules.Service.Persistence.Context;

public sealed class ServiceWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Service");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceWriteDbContext).Assembly);
        ServiceModelConfiguration.ConfigureReferences(modelBuilder);
    }

    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Domain.Entities.Service> Services => Set<Domain.Entities.Service>();
    public DbSet<ServiceMaterial> ServiceMaterials => Set<ServiceMaterial>();
    public DbSet<ServicePersonnel> ServicePersonnels => Set<ServicePersonnel>();
    public DbSet<ServiceTariff> ServiceTariffs => Set<ServiceTariff>();
    public DbSet<ServiceTariffItem> ServiceTariffItems => Set<ServiceTariffItem>();
}

public sealed class ServiceReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Service");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceReadDbContext).Assembly);
        ServiceModelConfiguration.ConfigureReferences(modelBuilder);
    }

    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Domain.Entities.Service> Services => Set<Domain.Entities.Service>();
    public DbSet<ServiceMaterial> ServiceMaterials => Set<ServiceMaterial>();
    public DbSet<ServicePersonnel> ServicePersonnels => Set<ServicePersonnel>();
    public DbSet<ServiceTariff> ServiceTariffs => Set<ServiceTariff>();
    public DbSet<ServiceTariffItem> ServiceTariffItems => Set<ServiceTariffItem>();
}

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

public sealed class ServiceWriteDbContextFactory
    : IDesignTimeDbContextFactory<ServiceWriteDbContext>
{
    public ServiceWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create Service migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new ServiceWriteDbContext(options);
    }
}
