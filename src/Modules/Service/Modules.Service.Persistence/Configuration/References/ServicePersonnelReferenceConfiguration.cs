using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modules.Service.Persistence.Context;

internal sealed class ServicePersonnelReference
{
    public long Id { get; set; }
}

internal sealed class ServicePersonnelReferenceConfiguration
    : IEntityTypeConfiguration<ServicePersonnelReference>
{
    public void Configure(EntityTypeBuilder<ServicePersonnelReference> builder)
    {
        builder.ToTable("Personnels", "Salon", table => table.ExcludeFromMigrations());
        builder.HasKey(personnel => personnel.Id);
    }
}
