using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Entities;

namespace Modules.Service.Persistence.Context;

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
