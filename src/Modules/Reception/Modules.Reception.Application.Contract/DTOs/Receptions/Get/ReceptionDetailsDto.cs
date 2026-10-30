using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.DTOs.Receptions.Get;

public sealed record ReceptionServiceMaterialDto(
    long Id,
    long MaterialId,
    string? MaterialName,
    string? BaseUnitName,
    string? BaseUnitSymbol,
    decimal Quantity,
    decimal? UnitCost,
    decimal? TotalCost,
    string? Description,
    bool MovementRegistered,
    bool MovementReversed);

public sealed record ReceptionServiceDto(
    long Id,
    long ServiceId,
    long ServiceTariffId,
    long? PersonnelId,
    string? PersonnelName,
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
    string? Description,
    long? CashRegisterId);

public sealed record ReceptionDetailsDto(
    long Id,
    long SalonId,
    long ReceptionNumber,
    long CustomerId,
    string CustomerName,
    string? CustomerMobilePhone,
    bool? CustomerStatus,
    long ReceptionistId,
    long? ServiceTariffId,
    long? AppointmentId,
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
