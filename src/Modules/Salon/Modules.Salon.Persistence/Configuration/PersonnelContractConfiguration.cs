using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Personnel.Domain.Entities;

namespace Modules.Personnel.Persistence.Configuration;

public sealed class PersonnelContractConfiguration
    : IEntityTypeConfiguration<PersonnelContract>
{
    public void Configure(EntityTypeBuilder<PersonnelContract> builder)
    {
        builder.ToTable("PersonnelContracts");
        builder.HasKey(contract => contract.Id);
        builder.Property(contract => contract.CooperationType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(contract => contract.FixedSalaryAmount).HasPrecision(18, 2);
        builder.Property(contract => contract.DefaultCommissionRate).HasPrecision(5, 2);
        builder.Property(contract => contract.EffectiveFrom).IsRequired();
        builder.Property(contract => contract.IsActive).IsRequired();
        builder.HasIndex(contract => new { contract.PersonnelId, contract.IsActive })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
        builder.HasIndex(contract => new { contract.PersonnelId, contract.EffectiveFrom });
    }
}
