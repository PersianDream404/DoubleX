using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Reception.Domain.Entities;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Persistence.Configuration;

public sealed class ReceptionConfiguration : IEntityTypeConfiguration<ReceptionEntity>
{
    public void Configure(EntityTypeBuilder<ReceptionEntity> builder)
    {
        builder.ToTable("Receptions");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ReceptionNumber).IsRequired();
        builder.Property(item => item.CustomerName).HasMaxLength(250).IsRequired();
        builder.Property(item => item.Notes).HasMaxLength(2000);
        builder.Property(item => item.ReceptionStatus).IsRequired();
        builder.Property(item => item.SubTotal).HasPrecision(18, 2);
        builder.Property(item => item.TotalDiscountAmount).HasPrecision(18, 2);
        builder.Property(item => item.TotalAmount).HasPrecision(18, 2);
        builder.Property(item => item.ReceptionFinancialStatus)
            .HasColumnName("FinancialStatus")
            .IsRequired();
        builder.Property(item => item.RowVersion).IsRowVersion();
        builder.Property(item => item.Revision).IsConcurrencyToken();
        builder.HasIndex(item => new { item.SalonId, item.ReceptionNumber }).IsUnique();
        builder.HasIndex(item => new { item.SalonId, item.CustomerId, item.ReceptionDate });
        builder.HasIndex(item => item.AppointmentId)
            .IsUnique()
            .HasFilter("[AppointmentId] IS NOT NULL");
        builder.HasMany(item => item.Services)
            .WithOne(service => service.Reception)
            .HasForeignKey(service => service.ReceptionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(item => item.Payments)
            .WithOne(payment => payment.Reception)
            .HasForeignKey(payment => payment.ReceptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ReceptionNumberSequence
{
    public long SalonId { get; set; }
    public long LastNumber { get; set; }
}

internal sealed class ReceptionNumberSequenceConfiguration
    : IEntityTypeConfiguration<ReceptionNumberSequence>
{
    public void Configure(EntityTypeBuilder<ReceptionNumberSequence> builder)
    {
        builder.ToTable("ReceptionNumberSequences");
        builder.HasKey(sequence => sequence.SalonId);
        builder.Property(sequence => sequence.SalonId).ValueGeneratedNever();
        builder.Property(sequence => sequence.LastNumber).IsRequired();
    }
}

public sealed class ReceptionServiceConfiguration : IEntityTypeConfiguration<ReceptionService>
{
    public void Configure(EntityTypeBuilder<ReceptionService> builder)
    {
        builder.ToTable("ReceptionServices");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ServiceName).HasMaxLength(250).IsRequired();
        builder.Property(item => item.Quantity).HasPrecision(18, 3);
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
        builder.Property(item => item.DiscountAmount).HasPrecision(18, 2);
        builder.Property(item => item.FinalAmount).HasPrecision(18, 2);
        builder.HasIndex(item => new { item.ReceptionId, item.ServiceId });
        builder.HasMany(item => item.Materials)
            .WithOne(material => material.ReceptionService)
            .HasForeignKey(material => material.ReceptionServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ReceptionServiceMaterialConfiguration
    : IEntityTypeConfiguration<ReceptionServiceMaterial>
{
    public void Configure(EntityTypeBuilder<ReceptionServiceMaterial> builder)
    {
        builder.ToTable("ReceptionServiceMaterials");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Quantity).HasPrecision(18, 3);
        builder.Property(item => item.UnitCost).HasPrecision(38, 8);
        builder.Property(item => item.TotalCost).HasPrecision(18, 2);
        builder.Property(item => item.IdempotencyKey).HasMaxLength(150).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(1000);
        builder.HasIndex(item => new { item.ReceptionServiceId, item.IdempotencyKey }).IsUnique();
        builder.HasIndex(item => item.MaterialId);
    }
}

public sealed class ReceptionPaymentConfiguration : IEntityTypeConfiguration<ReceptionPayment>
{
    public void Configure(EntityTypeBuilder<ReceptionPayment> builder)
    {
        builder.ToTable("ReceptionPayments");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Amount).HasPrecision(18, 2);
        builder.Property(item => item.PaymentMethod).HasMaxLength(50).IsRequired();
        builder.Property(item => item.PaymentStatus).IsRequired();
        builder.Property(item => item.TransactionReference).HasMaxLength(150);
        builder.Property(item => item.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(1000);
        builder.HasIndex(item => item.CashRegisterId);
        builder.HasIndex(item => new { item.ReceptionId, item.IdempotencyKey }).IsUnique();
        builder.HasIndex(item => new { item.ReceptionId, item.TransactionReference })
            .IsUnique()
            .HasFilter("[TransactionReference] IS NOT NULL");
    }
}

public sealed class CashRegisterPaymentOutboxConfiguration
    : IEntityTypeConfiguration<CashRegisterPaymentOutbox>
{
    public void Configure(EntityTypeBuilder<CashRegisterPaymentOutbox> builder)
    {
        builder.ToTable("CashRegisterPaymentOutbox");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(item => item.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(1000);
        builder.Property(item => item.LastError).HasMaxLength(2000);
        builder.Property(item => item.OutboxStatus).IsRequired();
        builder.HasIndex(item => new { item.OutboxStatus, item.NextAttemptAt, item.Id });
        builder.HasIndex(item => new { item.ReceptionId, item.IdempotencyKey }).IsUnique();
    }

    public sealed class PersonnelCommissionOutboxConfiguration
        : IEntityTypeConfiguration<PersonnelCommissionOutbox>
    {
        public void Configure(EntityTypeBuilder<PersonnelCommissionOutbox> builder)
        {
            builder.ToTable("PersonnelCommissionOutbox");
            builder.HasKey(item => item.Id);
            builder.Property(item => item.EventId).IsRequired();
            builder.Property(item => item.OutboxStatus).IsRequired();
            builder.Property(item => item.LastError).HasMaxLength(2000);
            builder.HasIndex(item => new { item.ReceptionId, item.ReceptionServiceId })
                .IsUnique();
            builder.HasIndex(item => new { item.OutboxStatus, item.NextAttemptAt, item.Id });
            builder.HasIndex(item => item.EventId).IsUnique();
        }
    }
}
