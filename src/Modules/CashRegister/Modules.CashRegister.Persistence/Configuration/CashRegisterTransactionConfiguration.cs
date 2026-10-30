using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.CashRegister.Domain.Entities;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Persistence.Configuration;

public sealed class CashRegisterTransactionConfiguration
    : IEntityTypeConfiguration<CashRegisterTransaction>
{
    public void Configure(EntityTypeBuilder<CashRegisterTransaction> builder)
    {
        builder.ToTable("CashRegisterTransactions");
        builder.HasKey(transaction => transaction.Id);
        builder.Property(transaction => transaction.Type).IsRequired();
        builder.Property(transaction => transaction.Amount).HasPrecision(18, 2);
        builder.Property(transaction => transaction.Description).HasMaxLength(1000);
        builder.Property(transaction => transaction.ReferenceType).HasMaxLength(50);
        builder.Property(transaction => transaction.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.HasIndex(transaction => new { transaction.SalonId, transaction.IdempotencyKey }).IsUnique();
        builder.HasIndex(transaction => new { transaction.CashRegisterId, transaction.OccurredAt });
        builder.HasIndex(transaction => new { transaction.SessionId, transaction.OccurredAt });
        builder.HasOne<CashRegisterEntity>()
            .WithMany()
            .HasForeignKey(transaction => transaction.CashRegisterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
