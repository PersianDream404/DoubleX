using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.CashRegister.Domain.Entities;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Persistence.Configuration;

public sealed class CashRegisterConfiguration : IEntityTypeConfiguration<CashRegisterEntity>
{
    public void Configure(EntityTypeBuilder<CashRegisterEntity> builder)
    {
        builder.ToTable("CashRegisters");
        builder.HasKey(register => register.Id);
        builder.Property(register => register.Name).HasMaxLength(150).IsRequired();
        builder.Property(register => register.Code).HasMaxLength(50).IsRequired();
        builder.Property(register => register.Description).HasMaxLength(1000);
        builder.HasIndex(register => new { register.SalonId, register.Code }).IsUnique();
        builder.HasIndex(register => new { register.SalonId, register.Status });
        builder.HasMany(register => register.Sessions)
            .WithOne()
            .HasForeignKey(session => session.CashRegisterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CashRegisterSessionConfiguration : IEntityTypeConfiguration<CashRegisterSession>
{
    public void Configure(EntityTypeBuilder<CashRegisterSession> builder)
    {
        builder.ToTable("CashRegisterSessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.OpeningBalance).HasPrecision(18, 2);
        builder.Property(session => session.ExpectedClosingBalance).HasPrecision(18, 2);
        builder.Property(session => session.ActualClosingBalance).HasPrecision(18, 2);
        builder.Property(session => session.Difference).HasPrecision(18, 2);
        builder.Property(session => session.SessionStatus).IsRequired();
        builder.Property(session => session.Revision).IsConcurrencyToken();
        builder.HasIndex(session => new { session.SalonId, session.CashRegisterId, session.SessionStatus })
            .IsUnique()
            .HasFilter("[SessionStatus] = 1");
        builder.HasMany(session => session.Transactions)
            .WithOne(transaction => transaction.Session)
            .HasForeignKey(transaction => transaction.SessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

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
