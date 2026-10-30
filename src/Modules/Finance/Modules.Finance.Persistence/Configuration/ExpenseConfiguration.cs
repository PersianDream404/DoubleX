using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Persistence.Configuration;

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
