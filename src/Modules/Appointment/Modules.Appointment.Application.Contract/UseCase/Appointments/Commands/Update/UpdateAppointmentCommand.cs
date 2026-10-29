using Framwork.Bus.Command;
using Modules.Appointment.Application.Contract.DTOs.Appointments;

namespace Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.Update;

public sealed record UpdateAppointmentCommand(
    long Id,
    long SalonId,
    UpdateAppointmentRequestDto Request) : ICommand<bool>;
