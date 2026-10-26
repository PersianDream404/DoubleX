namespace Modules.Reception.Application.Contract.DTOs.Receptions.Create;

public sealed record CreateReceptionRequestDto(
    long SalonId,
    long CustomerId,
    long ReceptionistId,
    DateTimeOffset ReceptionDate,
    string? Notes,
    long? AppointmentId);
