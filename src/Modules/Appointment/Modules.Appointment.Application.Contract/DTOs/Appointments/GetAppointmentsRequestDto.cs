using Framwork.PagedList;
using Modules.Appointment.Domain.Entities;

namespace Modules.Appointment.Application.Contract.DTOs.Appointments;

public sealed class GetAppointmentsRequestDto : PagedParamData
{
    public long SalonId { get; set; }
    public long? CustomerId { get; set; }
    public long? PersonnelId { get; set; }
    public long? ServiceId { get; set; }
    public AppointmentStatus? Status { get; set; }
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
}
