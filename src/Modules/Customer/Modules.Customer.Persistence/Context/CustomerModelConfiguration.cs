using Microsoft.EntityFrameworkCore;

namespace Modules.Customer.Persistence.Context;

internal static class CustomerModelConfiguration
{
    public static void ConfigureSalonEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerSalonReference>(builder =>
        {
            builder.ToTable("Salons", "Salon", table => table.ExcludeFromMigrations());
            builder.HasKey(salon => salon.Id);
        });
    }
}

internal sealed class CustomerSalonReference
{
    public long Id { get; set; }
}
