using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.CashRegister.Domain.Entities;
using SharedKernel.Constants;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Persistence.Context;

public sealed class CashRegisterWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("CashRegister");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CashRegisterWriteDbContext).Assembly);
    }

    public DbSet<CashRegisterEntity> CashRegisters => Set<CashRegisterEntity>();
    public DbSet<CashRegisterSession> Sessions => Set<CashRegisterSession>();
    public DbSet<CashRegisterTransaction> Transactions => Set<CashRegisterTransaction>();
}

public sealed class CashRegisterReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("CashRegister");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CashRegisterReadDbContext).Assembly);
    }

    public DbSet<CashRegisterEntity> CashRegisters => Set<CashRegisterEntity>();
    public DbSet<CashRegisterSession> Sessions => Set<CashRegisterSession>();
    public DbSet<CashRegisterTransaction> Transactions => Set<CashRegisterTransaction>();
}

public sealed class CashRegisterWriteDbContextFactory
    : IDesignTimeDbContextFactory<CashRegisterWriteDbContext>
{
    public CashRegisterWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create CashRegister migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new CashRegisterWriteDbContext(options);
    }
}
