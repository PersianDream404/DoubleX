using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.DTOs.Receptions.Get;

public sealed record ReceptionServiceMaterialDto(
    long Id,
    long MaterialId,
    decimal Quantity,
    decimal? UnitCost,
    string? Description,
    bool MovementRegistered,
    bool MovementReversed);

public sealed record ReceptionServiceDto(
    long Id,
    long ServiceId,
    long ServiceTariffId,
    long? PersonnelId,
    string ServiceName,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    decimal FinalAmount,
    ReceptionServiceStatus Status,
    IReadOnlyList<ReceptionServiceMaterialDto> Materials);

public sealed record ReceptionPaymentDto(
    long Id,
    decimal Amount,
    string PaymentMethod,
    DateTimeOffset PaidAt,
    ReceptionPaymentStatus Status,
    string? TransactionReference,
    string? Description);

public sealed record ReceptionDetailsDto(
    long Id,
    long SalonId,
    string ReceptionNumber,
    long CustomerId,
    string CustomerName,
    long ReceptionistId,
    long? ServiceTariffId,
    DateTimeOffset ReceptionDate,
    ReceptionStatus Status,
    decimal SubTotal,
    decimal TotalDiscountAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    ReceptionFinancialStatus FinancialStatus,
    string? Notes,
    IReadOnlyList<ReceptionServiceDto> Services,
    IReadOnlyList<ReceptionPaymentDto> Payments);
