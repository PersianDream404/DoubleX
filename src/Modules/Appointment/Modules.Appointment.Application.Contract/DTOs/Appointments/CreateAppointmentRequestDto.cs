using Modules.Appointment.Domain.Entities;

namespace Modules.Appointment.Application.Contract.DTOs.Appointments;

public sealed record CreateAppointmentRequestDto(
    Guid IdempotencyKey,
    long CustomerId,
    long ServiceId,
    long ServiceTariffId,
    long PersonnelId,
    DateTimeOffset StartsAt,
    decimal DiscountAmount,
    string? Notes,
    AppointmentNotificationChannel NotificationChannel);
