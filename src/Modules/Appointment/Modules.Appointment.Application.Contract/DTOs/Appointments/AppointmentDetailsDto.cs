using Modules.Appointment.Domain.Entities;

namespace Modules.Appointment.Application.Contract.DTOs.Appointments;

public sealed record AppointmentDetailsDto(
    long Id,
    long SalonId,
    long CustomerId,
    string CustomerName,
    long ServiceId,
    string ServiceName,
    long ServiceTariffId,
    long PersonnelId,
    string PersonnelName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int DurationMinutes,
    string? Notes,
    decimal BaseAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    AppointmentNotificationChannel NotificationChannel,
    AppointmentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
