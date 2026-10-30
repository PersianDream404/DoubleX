using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
