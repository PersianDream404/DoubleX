namespace Modules.Reception.Application.Contract.DTOs.Receptions.Create;

public sealed record CreateReceptionRequestDto(
    long SalonId,
    long CustomerId,
    long ReceptionistId,
    long ServiceTariffId,
    DateTimeOffset ReceptionDate,
    string? Notes,
    long? AppointmentId,
    IReadOnlyList<CreateReceptionServiceDto> Services,
    IReadOnlyList<CreateReceptionPaymentDto> Payments);

public sealed record CreateReceptionServiceDto(
    long ServiceTariffId,
    long ServiceId,
    long? PersonnelId,
    decimal Quantity,
    decimal DiscountAmount,
    IReadOnlyList<CreateReceptionServiceMaterialDto> Materials);

public sealed record CreateReceptionServiceMaterialDto(
    long MaterialId,
    decimal Quantity,
    string IdempotencyKey,
    string? Description);

public sealed record CreateReceptionPaymentDto(
    decimal Amount,
    string PaymentMethod,
    DateTimeOffset PaidAt,
    string? TransactionReference,
    string IdempotencyKey,
    string? Description,
    long? CashRegisterId);
