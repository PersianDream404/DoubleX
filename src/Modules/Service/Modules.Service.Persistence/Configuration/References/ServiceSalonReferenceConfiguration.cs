using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modules.Service.Persistence.Context;

internal sealed class ServiceSalonReference
{
    public long Id { get; set; }
}

internal sealed class ServiceSalonReferenceConfiguration
    : IEntityTypeConfiguration<ServiceSalonReference>
{
    public void Configure(EntityTypeBuilder<ServiceSalonReference> builder)
    {
        builder.ToTable("Salons", "Salon", table => table.ExcludeFromMigrations());
        builder.HasKey(salon => salon.Id);
    }
}
