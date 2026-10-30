using AppointmentEntity = Modules.Appointment.Domain.Entities.Appointment;

namespace Modules.Appointment.Domain.Interfaces;

public interface IAppointmentCommandRepository
{
    Task<AppointmentEntity?> GetTrackedAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken);

    Task<AppointmentEntity?> GetByIdempotencyKeyAsync(
        long salonId,
        Guid idempotencyKey,
        CancellationToken cancellationToken);

    Task<AppointmentWriteResult> AddIfAvailableAsync(
        AppointmentEntity appointment,
        CancellationToken cancellationToken);

    Task<bool> IsAvailableAsync(
        long salonId,
        long personnelId,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        long? excludingAppointmentId,
        CancellationToken cancellationToken);

    Task<bool> UpdateScheduleIfAvailableAsync(
        AppointmentEntity appointment,
        CancellationToken cancellationToken);

    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}
