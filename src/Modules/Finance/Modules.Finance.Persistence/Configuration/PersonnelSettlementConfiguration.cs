using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Persistence.Configuration;

public sealed class PersonnelSettlementConfiguration
    : IEntityTypeConfiguration<PersonnelSettlement>
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
