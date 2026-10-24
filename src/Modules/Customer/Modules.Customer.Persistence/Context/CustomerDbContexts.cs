using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;

namespace Modules.Customer.Persistence.Context;

public sealed class CustomerWriteDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerWriteDbContext).Assembly);
        CustomerModelConfiguration.ConfigureSalonEntity(modelBuilder);
    }

    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
}

public sealed class CustomerReadDbContext(DbContextOptions<BaseDbContext> options)
    : BaseDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerReadDbContext).Assembly);
        CustomerModelConfiguration.ConfigureSalonEntity(modelBuilder);
    }

    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();

}

internal static class CustomerModelConfiguration
{
    public static void ConfigureSalonEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerSalonReference>(builder =>
        {
            builder.ToTable("Salons");
            builder.HasKey(salon => salon.Id);
        });
    }
}

internal sealed class CustomerSalonReference
{
    public long Id { get; set; }
}

public sealed class CustomerWriteDbContextFactory
    : IDesignTimeDbContextFactory<CustomerWriteDbContext>
{
    public CustomerWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create Customer migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new CustomerWriteDbContext(options);
    }
}
