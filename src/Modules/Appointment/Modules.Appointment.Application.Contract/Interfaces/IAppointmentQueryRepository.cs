using Framwork.PagedList;
using Modules.Appointment.Application.Contract.DTOs.Appointments;

namespace Modules.Appointment.Application.Contract.Interfaces;

public interface IAppointmentQueryRepository
{
    Task<PagedList<AppointmentListItemDto>> GetAllAsync(
        GetAppointmentsRequestDto request,
        CancellationToken cancellationToken);

    Task<AppointmentDetailsDto?> GetByIdAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken);
}
