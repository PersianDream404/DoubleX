using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Configuration.ServicePersonnels;

public sealed class ServicePersonnelConfiguration : IEntityTypeConfiguration<ServicePersonnel>
{
    public void Configure(EntityTypeBuilder<ServicePersonnel> builder)
    {
        builder.ToTable("ServicePersonnels");
        builder.HasKey(item => item.Id);
        builder.HasAlternateKey(item => new { item.SalonId, item.Id });
        builder.HasIndex(item => new { item.SalonId, item.ServiceId, item.PersonnelId })
            .IsUnique()
            .HasFilter("[Deleted] = 0");
        builder.HasIndex(item => item.PersonnelId);
        builder.HasOne(item => item.Service)
            .WithMany(service => service.Personnels)
            .HasForeignKey(item => new { item.SalonId, item.ServiceId })
            .HasPrincipalKey(service => new { service.SalonId, service.Id })
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder.HasOne<ServicePersonnelReference>()
            .WithMany()
            .HasForeignKey(item => item.PersonnelId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
