using Framwork.Bus.Command;
using Modules.Appointment.Application.Contract.DTOs.Appointments;

namespace Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.Create;

public sealed record CreateAppointmentCommand(
    long SalonId,
    CreateAppointmentRequestDto Request) : ICommand<long>;
