using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.DTOs.Receptions.GetAll;

public sealed record ReceptionListItemDto(
    long Id,
    long SalonId,
    string ReceptionNumber,
    long CustomerId,
    string CustomerName,
    long ReceptionistId,
    long? ServiceTariffId,
    DateTimeOffset ReceptionDate,
    ReceptionStatus Status,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    ReceptionFinancialStatus FinancialStatus);
