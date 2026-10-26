using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Configuration.ServiceCategories;

public sealed class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.ToTable("ServiceCategories");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Name).HasMaxLength(200).IsRequired();
        builder.Property(category => category.Description).HasMaxLength(1000);
        builder.Property(category => category.Status).HasColumnName("IsActive").IsRequired();
        builder.Ignore(category => category.IsActive);
        builder.HasAlternateKey(category => new { category.SalonId, category.Id });
        builder.HasIndex(category => new { category.SalonId, category.Name }).IsUnique();
        builder.HasOne<ServiceSalonReference>()
            .WithMany()
            .HasForeignKey(category => category.SalonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
