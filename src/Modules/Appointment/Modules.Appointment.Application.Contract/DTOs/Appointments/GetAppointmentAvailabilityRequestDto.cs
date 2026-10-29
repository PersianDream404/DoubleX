namespace Modules.Appointment.Application.Contract.DTOs.Appointments;

public sealed record GetAppointmentAvailabilityRequestDto(
    long ServiceId,
    long ServiceTariffId,
    long PersonnelId,
    DateTimeOffset StartsAt);

public sealed record AppointmentAvailabilityDto(
    bool IsAvailable,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int DurationMinutes,
    decimal BaseAmount);
