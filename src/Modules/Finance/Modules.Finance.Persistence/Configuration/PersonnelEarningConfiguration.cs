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
