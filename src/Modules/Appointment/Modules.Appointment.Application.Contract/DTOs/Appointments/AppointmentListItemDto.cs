using Modules.Appointment.Domain.Entities;

namespace Modules.Appointment.Application.Contract.DTOs.Appointments;

public sealed record AppointmentListItemDto(
    long Id,
    long CustomerId,
    string CustomerName,
    long ServiceId,
    string ServiceName,
    long PersonnelId,
    string PersonnelName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    decimal FinalAmount,
    AppointmentStatus Status);
