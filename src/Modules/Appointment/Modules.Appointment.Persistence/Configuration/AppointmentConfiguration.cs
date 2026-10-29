using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Appointment.Domain.Entities;
using AppointmentEntity = Modules.Appointment.Domain.Entities.Appointment;

namespace Modules.Appointment.Persistence.Configuration;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<AppointmentEntity>
{
    public void Configure(EntityTypeBuilder<AppointmentEntity> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(appointment => appointment.Id);
        builder.Property(appointment => appointment.CustomerName).HasMaxLength(250).IsRequired();
        builder.Property(appointment => appointment.ServiceName).HasMaxLength(250).IsRequired();
        builder.Property(appointment => appointment.PersonnelName).HasMaxLength(250).IsRequired();
        builder.Property(appointment => appointment.Notes).HasMaxLength(2000);
        builder.Property(appointment => appointment.BaseAmount).HasPrecision(18, 2);
        builder.Property(appointment => appointment.DiscountAmount).HasPrecision(18, 2);
        builder.Property(appointment => appointment.FinalAmount).HasPrecision(18, 2);
        builder.Property(appointment => appointment.Status).IsRequired();
        builder.Property(appointment => appointment.NotificationChannel).IsRequired();
        builder.Property(appointment => appointment.IdempotencyKey).IsRequired();
        builder.Property(appointment => appointment.RowVersion).IsRowVersion();
        builder.HasIndex(appointment => new { appointment.SalonId, appointment.IdempotencyKey })
            .IsUnique();
        builder.HasIndex(appointment => new
        {
            appointment.SalonId,
            appointment.PersonnelId,
            appointment.StartsAt
        });
        builder.HasIndex(appointment => new
        {
            appointment.SalonId,
            appointment.CustomerId,
            appointment.StartsAt
        });
    }
}
