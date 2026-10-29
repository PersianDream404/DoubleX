using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Domain.ReadModels;

public sealed record ReceptionListReadModel(
    long Id,
    long SalonId,
    string ReceptionNumber,
    long CustomerId,
    string CustomerName,
    long ReceptionistId,
    long? ServiceTariffId,
    long? AppointmentId,
    DateTimeOffset ReceptionDate,
    ReceptionStatus Status,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    ReceptionFinancialStatus FinancialStatus);

public sealed record ReceptionServiceMaterialReadModel(
    long Id,
    long MaterialId,
    decimal Quantity,
    decimal? UnitCost,
    string? Description,
    bool MovementRegistered,
    bool MovementReversed);

public sealed record ReceptionServiceReadModel(
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
    IReadOnlyList<ReceptionServiceMaterialReadModel> Materials);

public sealed record ReceptionPaymentReadModel(
    long Id,
    decimal Amount,
    string PaymentMethod,
    DateTimeOffset PaidAt,
    ReceptionPaymentStatus Status,
    string? TransactionReference,
    string? Description,
    long? CashRegisterId);

public sealed record ReceptionDetailsReadModel(
    long Id,
    long SalonId,
    string ReceptionNumber,
    long CustomerId,
    string CustomerName,
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
    IReadOnlyList<ReceptionServiceReadModel> Services,
    IReadOnlyList<ReceptionPaymentReadModel> Payments);
