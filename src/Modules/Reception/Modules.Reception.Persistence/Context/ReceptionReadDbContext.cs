using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.Reception.Domain.Entities;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Persistence.Context;

public sealed class ReceptionReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Reception");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReceptionReadDbContext).Assembly);
    }

    public DbSet<ReceptionEntity> Receptions => Set<ReceptionEntity>();
    public DbSet<ReceptionService> ReceptionServices => Set<ReceptionService>();
    public DbSet<ReceptionServiceMaterial> ReceptionServiceMaterials => Set<ReceptionServiceMaterial>();
    public DbSet<ReceptionPayment> ReceptionPayments => Set<ReceptionPayment>();
    public DbSet<CashRegisterPaymentOutbox> CashRegisterPaymentOutbox => Set<CashRegisterPaymentOutbox>();
    public DbSet<PersonnelCommissionOutbox> PersonnelCommissionOutbox => Set<PersonnelCommissionOutbox>();
}
