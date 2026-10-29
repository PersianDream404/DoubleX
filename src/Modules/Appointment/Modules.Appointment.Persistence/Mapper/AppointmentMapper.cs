using System.Linq.Expressions;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Domain.Entities;
using AppointmentEntity = Modules.Appointment.Domain.Entities.Appointment;

namespace Modules.Appointment.Persistence.Mapper;

public static class AppointmentMapper
{
    public static Expression<Func<AppointmentEntity, AppointmentListItemDto>> ToListItemDto() =>
        appointment => new AppointmentListItemDto(
            appointment.Id,
            appointment.CustomerId,
            appointment.CustomerName,
            appointment.ServiceId,
            appointment.ServiceName,
            appointment.PersonnelId,
            appointment.PersonnelName,
            appointment.StartsAt,
            appointment.EndsAt,
            appointment.FinalAmount,
            appointment.AppointmentStatus);

    public static Expression<Func<AppointmentEntity, AppointmentDetailsDto>> ToDetailsDto() =>
        appointment => new AppointmentDetailsDto(
            appointment.Id,
            appointment.SalonId,
            appointment.CustomerId,
            appointment.CustomerName,
            appointment.ServiceId,
            appointment.ServiceName,
            appointment.ServiceTariffId,
            appointment.PersonnelId,
            appointment.PersonnelName,
            appointment.StartsAt,
            appointment.EndsAt,
            appointment.DurationMinutes,
            appointment.Notes,
            appointment.BaseAmount,
            appointment.DiscountAmount,
            appointment.FinalAmount,
            appointment.NotificationChannel,
            appointment.AppointmentStatus,
            appointment.CreatedAt,
            appointment.UpdatedAt);
}
