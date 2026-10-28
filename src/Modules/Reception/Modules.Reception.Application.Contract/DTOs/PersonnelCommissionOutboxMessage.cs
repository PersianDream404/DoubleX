namespace Modules.Reception.Application.Contract.DTOs;

public sealed record PersonnelCommissionOutboxMessage(
    long Id,
    Guid EventId,
    long SalonId,
    long ReceptionId,
    long ReceptionServiceId);
