using Framwork.Bus.Command;
using Modules.Appointment.Domain.Entities;

namespace Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.ChangeStatus;

public sealed record ChangeAppointmentStatusCommand(
    long Id,
    long SalonId,
    AppointmentStatus Status) : ICommand<bool>;
