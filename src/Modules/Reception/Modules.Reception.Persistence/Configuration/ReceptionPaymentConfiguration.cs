using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Persistence.Configuration;

public sealed class ReceptionPaymentConfiguration : IEntityTypeConfiguration<ReceptionPayment>
{
    public void Configure(EntityTypeBuilder<ReceptionPayment> builder)
    {
        builder.ToTable("ReceptionPayments");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Amount).HasPrecision(18, 2);
        builder.Property(item => item.PaymentMethod).HasMaxLength(50).IsRequired();
        builder.Property(item => item.PaymentStatus).IsRequired();
        builder.Property(item => item.TransactionReference).HasMaxLength(150);
        builder.Property(item => item.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(1000);
        builder.HasIndex(item => item.CashRegisterId);
        builder.HasIndex(item => new { item.ReceptionId, item.IdempotencyKey }).IsUnique();
        builder.HasIndex(item => new { item.ReceptionId, item.TransactionReference })
            .IsUnique()
            .HasFilter("[TransactionReference] IS NOT NULL");
    }
}
