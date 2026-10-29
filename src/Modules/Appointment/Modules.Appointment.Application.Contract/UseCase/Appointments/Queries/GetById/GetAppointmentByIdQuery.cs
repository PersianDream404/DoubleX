using Framwork.Bus.Query;
using Modules.Appointment.Application.Contract.DTOs.Appointments;

namespace Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetById;

public sealed record GetAppointmentByIdQuery(long Id, long SalonId)
    : IQuery<AppointmentDetailsDto>;
