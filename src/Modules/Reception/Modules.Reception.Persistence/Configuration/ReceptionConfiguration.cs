using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Persistence.Configuration;

public sealed class ReceptionConfiguration : IEntityTypeConfiguration<ReceptionEntity>
{
    public void Configure(EntityTypeBuilder<ReceptionEntity> builder)
    {
        builder.ToTable("Receptions");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ReceptionNumber).IsRequired();
        builder.Property(item => item.CustomerName).HasMaxLength(250).IsRequired();
        builder.Property(item => item.Notes).HasMaxLength(2000);
        builder.Property(item => item.ReceptionStatus).IsRequired();
        builder.Property(item => item.SubTotal).HasPrecision(18, 2);
        builder.Property(item => item.TotalDiscountAmount).HasPrecision(18, 2);
        builder.Property(item => item.TotalAmount).HasPrecision(18, 2);
        builder.Property(item => item.ReceptionFinancialStatus)
            .HasColumnName("FinancialStatus")
            .IsRequired();
        builder.Property(item => item.RowVersion).IsRowVersion();
        builder.Property(item => item.Revision).IsConcurrencyToken();
        builder.HasIndex(item => new { item.SalonId, item.ReceptionNumber }).IsUnique();
        builder.HasIndex(item => new { item.SalonId, item.CustomerId, item.ReceptionDate });
        builder.HasIndex(item => item.AppointmentId)
            .IsUnique()
            .HasFilter("[AppointmentId] IS NOT NULL");
        builder.HasMany(item => item.Services)
            .WithOne(service => service.Reception)
            .HasForeignKey(service => service.ReceptionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(item => item.Payments)
            .WithOne(payment => payment.Reception)
            .HasForeignKey(payment => payment.ReceptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
