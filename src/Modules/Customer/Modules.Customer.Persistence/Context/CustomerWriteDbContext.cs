using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;

namespace Modules.Customer.Persistence.Context;

public sealed class CustomerWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Customer");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerWriteDbContext).Assembly);
        CustomerModelConfiguration.ConfigureSalonEntity(modelBuilder);
    }

    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
}
