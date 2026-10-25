using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Persistence.Context;

public sealed class InventoryWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryWriteDbContext).Assembly);
    }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
}

public sealed class InventoryReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryReadDbContext).Assembly);
    }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
}

public sealed class InventoryWriteDbContextFactory
    : IDesignTimeDbContextFactory<InventoryWriteDbContext>
{
    public InventoryWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create Inventory migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new InventoryWriteDbContext(options);
    }
}
