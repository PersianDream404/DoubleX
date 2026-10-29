using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Appointment.Application.Contract.DTOs.Appointments;

namespace Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetAll;

public sealed record GetAppointmentsQuery(GetAppointmentsRequestDto Request)
    : IQuery<PagedList<AppointmentListItemDto>>;
