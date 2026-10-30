using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Persistence.Configuration;

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
