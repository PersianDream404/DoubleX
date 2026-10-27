namespace Modules.Reception.Application.Contract.DTOs.Receptions.Create;

public sealed record CreateReceptionRequestDto(
    long SalonId,
    long CustomerId,
    long ReceptionistId,
    long ServiceTariffId,
    DateTimeOffset ReceptionDate,
    string? Notes,
    long? AppointmentId);
