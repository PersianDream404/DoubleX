namespace Modules.Appointment.Application.Contract.DTOs.Appointments;

public sealed record UpdateAppointmentRequestDto(
    DateTimeOffset StartsAt,
    long PersonnelId,
    string? Notes);
