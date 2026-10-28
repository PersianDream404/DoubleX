using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Persistence.Context;

public sealed class FinanceWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceWriteDbContext).Assembly);
    }

    public DbSet<PersonnelEarning> PersonnelEarnings => Set<PersonnelEarning>();
    public DbSet<PersonnelSettlement> PersonnelSettlements => Set<PersonnelSettlement>();
    public DbSet<PersonnelSettlementItem> PersonnelSettlementItems => Set<PersonnelSettlementItem>();
    public DbSet<Expense> Expenses => Set<Expense>();
}

public sealed class FinanceReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceReadDbContext).Assembly);
    }

    public DbSet<PersonnelEarning> PersonnelEarnings => Set<PersonnelEarning>();
    public DbSet<PersonnelSettlement> PersonnelSettlements => Set<PersonnelSettlement>();
    public DbSet<PersonnelSettlementItem> PersonnelSettlementItems => Set<PersonnelSettlementItem>();
    public DbSet<Expense> Expenses => Set<Expense>();
}

public sealed class FinanceWriteDbContextFactory
    : IDesignTimeDbContextFactory<FinanceWriteDbContext>
{
    public FinanceWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create Finance migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new FinanceWriteDbContext(options);
    }
}
