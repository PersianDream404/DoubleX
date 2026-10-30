using Framwork.PagedList;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Domain.ReadModels;

public sealed record ReceptionReportFilter(
    long SalonId,
    DateTimeOffset? FromDate,
    DateTimeOffset? ToDateExclusive,
    string? CustomerSearch,
    IReadOnlyCollection<long>? CustomerIds,
    long? CustomerId,
    long? ReceptionNumber,
    long? PersonnelId,
    long? ServiceId,
    long? MaterialId,
    ReceptionStatus? ReceptionStatus,
    ReceptionFinancialStatus? FinancialStatus,
    ReceptionServiceStatus? ServiceStatus,
    bool? HasAppointment,
    decimal? MinimumRemainingAmount,
    decimal? MaximumRemainingAmount,
    int PageNumber,
    int PageSize,
    bool OldestFirst);

public sealed record ReceptionReportTrendReadModel(
    DateOnly Date,
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount);

public sealed record ReceptionReportPaymentMethodReadModel(string PaymentMethod, decimal Amount);

public sealed record ReceptionReportSummaryReadModel(
    int ReceptionCount,
    int CompletedReceptionCount,
    int InProgressReceptionCount,
    int CancelledReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    decimal MaterialChargeAmount,
    IReadOnlyList<ReceptionReportTrendReadModel> Trend,
    IReadOnlyList<ReceptionReportPaymentMethodReadModel> PaymentMethods);

public sealed record ReceptionReportReceptionReadModel(
    long Id,
    long ReceptionNumber,
    long CustomerId,
    string CustomerName,
    long ReceptionistId,
    DateTimeOffset ReceptionDate,
    ReceptionStatus Status,
    ReceptionFinancialStatus FinancialStatus,
    int ServiceCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount,
    decimal RemainingAmount);

public sealed record ReceptionReportTotalsReadModel(
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount,
    decimal RemainingAmount);

public sealed record ReceptionReportPersonnelReadModel(
    long PersonnelId,
    int CompletedServiceCount,
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount);

public sealed record ReceptionReportServiceReadModel(
    long ServiceId,
    string ServiceName,
    int CompletedServiceCount,
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal AverageFinalAmount);

public sealed record ReceptionReportDebtReadModel(
    long ReceptionId,
    long ReceptionNumber,
    long CustomerId,
    string CustomerName,
    DateTimeOffset ReceptionDate,
    ReceptionStatus Status,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount);

public sealed record ReceptionReportDebtTotalsReadModel(
    int ReceptionCount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount);

public sealed record ReceptionReportDiscountReadModel(
    long ReceptionId,
    long ReceptionNumber,
    long CustomerId,
    string CustomerName,
    DateTimeOffset ReceptionDate,
    long ServiceId,
    string ServiceName,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount);

public sealed record ReceptionReportDiscountTotalsReadModel(
    int ServiceCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount);

public sealed record ReceptionReportMaterialReadModel(
    long MaterialId,
    long ServiceId,
    string ServiceName,
    int UsageCount,
    decimal Quantity,
    decimal? TotalCost);

public sealed record ReceptionReportMaterialTotalsReadModel(
    int UsageCount,
    decimal Quantity,
    decimal TotalCost);

public sealed record ReceptionReportPageReadModel<TItem, TTotals>(
    PagedList<TItem> Page,
    TTotals Totals);
