using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modules.Reception.Domain.Entities;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;
using SharedKernel.Constants;

namespace Modules.Reception.Persistence.Context;

public sealed class ReceptionWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReceptionWriteDbContext).Assembly);
    }

    public DbSet<ReceptionEntity> Receptions => Set<ReceptionEntity>();
    public DbSet<ReceptionService> ReceptionServices => Set<ReceptionService>();
    public DbSet<ReceptionServiceMaterial> ReceptionServiceMaterials => Set<ReceptionServiceMaterial>();
    public DbSet<ReceptionPayment> ReceptionPayments => Set<ReceptionPayment>();
    public DbSet<CashRegisterPaymentOutbox> CashRegisterPaymentOutbox => Set<CashRegisterPaymentOutbox>();
    public DbSet<PersonnelCommissionOutbox> PersonnelCommissionOutbox => Set<PersonnelCommissionOutbox>();
}

public sealed class ReceptionReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReceptionReadDbContext).Assembly);
    }

    public DbSet<ReceptionEntity> Receptions => Set<ReceptionEntity>();
    public DbSet<ReceptionService> ReceptionServices => Set<ReceptionService>();
    public DbSet<ReceptionServiceMaterial> ReceptionServiceMaterials => Set<ReceptionServiceMaterial>();
    public DbSet<ReceptionPayment> ReceptionPayments => Set<ReceptionPayment>();
    public DbSet<CashRegisterPaymentOutbox> CashRegisterPaymentOutbox => Set<CashRegisterPaymentOutbox>();
    public DbSet<PersonnelCommissionOutbox> PersonnelCommissionOutbox => Set<PersonnelCommissionOutbox>();
}

public sealed class ReceptionWriteDbContextFactory
    : IDesignTimeDbContextFactory<ReceptionWriteDbContext>
{
    public ReceptionWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create Reception migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new ReceptionWriteDbContext(options);
    }
}
