using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Persistence.Context;

public sealed class InventoryReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Inventory");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryReadDbContext).Assembly);
    }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
}
