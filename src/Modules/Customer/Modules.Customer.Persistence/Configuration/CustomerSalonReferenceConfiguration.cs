using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modules.Customer.Persistence.Context;

internal sealed class CustomerSalonReference
{
    public long Id { get; set; }
}

internal sealed class CustomerSalonReferenceConfiguration
    : IEntityTypeConfiguration<CustomerSalonReference>
{
    public void Configure(EntityTypeBuilder<CustomerSalonReference> builder)
    {
        builder.ToTable("Salons", "Salon", table => table.ExcludeFromMigrations());
        builder.HasKey(salon => salon.Id);
    }
}
