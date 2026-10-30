using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Persistence.Configuration;

public sealed class CashRegisterSessionConfiguration
    : IEntityTypeConfiguration<CashRegisterSession>
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
        builder.HasIndex(session => new
            {
                session.SalonId,
                session.CashRegisterId,
                session.SessionStatus
            })
            .IsUnique()
            .HasFilter("[SessionStatus] = 1");
        builder.HasMany(session => session.Transactions)
            .WithOne(transaction => transaction.Session)
            .HasForeignKey(transaction => transaction.SessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
