using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Persistence.Configuration;

public sealed class CashRegisterPaymentOutboxConfiguration
    : IEntityTypeConfiguration<CashRegisterPaymentOutbox>
{
    public void Configure(EntityTypeBuilder<CashRegisterPaymentOutbox> builder)
    {
        builder.ToTable("CashRegisterPaymentOutbox");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(item => item.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(1000);
        builder.Property(item => item.LastError).HasMaxLength(2000);
        builder.Property(item => item.OutboxStatus).IsRequired();
        builder.HasIndex(item => new { item.OutboxStatus, item.NextAttemptAt, item.Id });
        builder.HasIndex(item => new { item.ReceptionId, item.IdempotencyKey }).IsUnique();
    }
}
