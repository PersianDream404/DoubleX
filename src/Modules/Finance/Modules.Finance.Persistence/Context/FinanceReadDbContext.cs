using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Persistence.Context;

public sealed class FinanceReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Finance");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceReadDbContext).Assembly);
    }

    public DbSet<PersonnelEarning> PersonnelEarnings => Set<PersonnelEarning>();
    public DbSet<PersonnelSettlement> PersonnelSettlements => Set<PersonnelSettlement>();
    public DbSet<PersonnelSettlementItem> PersonnelSettlementItems => Set<PersonnelSettlementItem>();
    public DbSet<Expense> Expenses => Set<Expense>();
}
