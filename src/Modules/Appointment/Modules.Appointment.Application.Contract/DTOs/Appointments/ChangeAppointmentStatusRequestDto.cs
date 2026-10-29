using Modules.Appointment.Domain.Entities;

namespace Modules.Appointment.Application.Contract.DTOs.Appointments;

public sealed record ChangeAppointmentStatusRequestDto(AppointmentStatus Status);
