using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.CashRegister.Domain.Entities;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Persistence.Context;

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
