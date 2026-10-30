namespace Modules.Appointment.Domain.Interfaces;

public enum AppointmentWriteResult
{
    Created,
    ExistingRequest,
    IdempotencyKeyReused,
    TimeConflict
}
