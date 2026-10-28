using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Persistence.Configuration;

public sealed class PersonnelEarningConfiguration : IEntityTypeConfiguration<PersonnelEarning>
{
    public void Configure(EntityTypeBuilder<PersonnelEarning> builder)
    {
        builder.ToTable("FinancePersonnelEarnings");
        builder.HasKey(earning => earning.Id);
        builder.Property(earning => earning.EarningType).IsRequired();
        builder.Property(earning => earning.CalculationBaseAmount).HasPrecision(18, 2);
        builder.Property(earning => earning.CommissionRate).HasPrecision(5, 2);
        builder.Property(earning => earning.Amount).HasPrecision(18, 2);
        builder.Property(earning => earning.Description).HasMaxLength(1000);
        builder.Property(earning => earning.IdempotencyKey).HasMaxLength(150).IsRequired();
        builder.HasIndex(earning => new { earning.SalonId, earning.IdempotencyKey }).IsUnique();
        builder.HasIndex(earning => new
        {
            earning.SalonId,
            earning.PersonnelId,
            earning.EarningType,
            earning.OccurredAt
        });
        builder.HasIndex(earning => new { earning.SalonId, earning.ReceptionServiceId })
            .IsUnique()
            .HasFilter("[ReceptionServiceId] IS NOT NULL");
    }
}

public sealed class PersonnelSettlementConfiguration : IEntityTypeConfiguration<PersonnelSettlement>
{
    public void Configure(EntityTypeBuilder<PersonnelSettlement> builder)
    {
        builder.ToTable("FinancePersonnelSettlements");
        builder.HasKey(settlement => settlement.Id);
        builder.Property(settlement => settlement.EarningType).IsRequired();
        builder.Property(settlement => settlement.Amount).HasPrecision(18, 2);
        builder.Property(settlement => settlement.PaymentMethod).HasMaxLength(50).IsRequired();
        builder.Property(settlement => settlement.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(settlement => settlement.Description).HasMaxLength(1000);
        builder.HasIndex(settlement => new { settlement.SalonId, settlement.IdempotencyKey }).IsUnique();
        builder.HasIndex(settlement => new
        {
            settlement.SalonId,
            settlement.PersonnelId,
            settlement.EarningType,
            settlement.PaidAt
        });
        builder.HasMany(settlement => settlement.Items)
            .WithOne(item => item.Settlement)
            .HasForeignKey(item => item.SettlementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PersonnelSettlementItemConfiguration
    : IEntityTypeConfiguration<PersonnelSettlementItem>
{
    public void Configure(EntityTypeBuilder<PersonnelSettlementItem> builder)
    {
        builder.ToTable("FinancePersonnelSettlementItems");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Amount).HasPrecision(18, 2);
        builder.HasIndex(item => new { item.SettlementId, item.PersonnelEarningId }).IsUnique();
        builder.HasOne(item => item.Earning)
            .WithMany()
            .HasForeignKey(item => item.PersonnelEarningId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("FinanceExpenses");
        builder.HasKey(expense => expense.Id);
        builder.Property(expense => expense.Category).HasMaxLength(100).IsRequired();
        builder.Property(expense => expense.Amount).HasPrecision(18, 2);
        builder.Property(expense => expense.Description).HasMaxLength(1000);
        builder.Property(expense => expense.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(expense => expense.CancellationReason).HasMaxLength(500);
        builder.HasIndex(expense => new { expense.SalonId, expense.IdempotencyKey }).IsUnique();
        builder.HasIndex(expense => new { expense.SalonId, expense.OccurredAt, expense.Status });
    }
}
