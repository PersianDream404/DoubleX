using Framwork.Bus.Query;
using Modules.Appointment.Application.Contract.DTOs.Appointments;

namespace Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetAvailability;

public sealed record GetAppointmentAvailabilityQuery(
    long SalonId,
    GetAppointmentAvailabilityRequestDto Request)
    : IQuery<AppointmentAvailabilityDto>;
