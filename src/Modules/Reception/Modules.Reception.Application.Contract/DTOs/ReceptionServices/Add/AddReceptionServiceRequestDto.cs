namespace Modules.Reception.Application.Contract.DTOs.ReceptionServices.Add;

public sealed record AddReceptionServiceRequestDto(
    long SalonId,
    long ServiceTariffId,
    long ServiceId,
    long? PersonnelId,
    decimal Quantity,
    decimal DiscountAmount);

public sealed record ChangeReceptionServiceStatusRequestDto(
    long SalonId,
    Modules.Reception.Domain.Entities.ReceptionServiceStatus Status);
