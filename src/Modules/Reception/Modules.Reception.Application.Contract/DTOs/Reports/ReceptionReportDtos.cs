using Framwork.PagedList;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.DTOs.Reports;

public sealed record ReceptionReportTrendDto(
    DateOnly Date,
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount);

public sealed record ReceptionReportPaymentMethodDto(string PaymentMethod, decimal Amount);

public sealed record ReceptionReportSummaryDto(
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
    IReadOnlyList<ReceptionReportTrendDto> Trend,
    IReadOnlyList<ReceptionReportPaymentMethodDto> PaymentMethods);

public sealed record ReceptionReportReceptionDto(
    long Id,
    long ReceptionNumber,
    long CustomerId,
    string CustomerName,
    string? CustomerMobilePhone,
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

public sealed record ReceptionReportTotalsDto(
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount,
    decimal RemainingAmount);

public sealed record ReceptionReportPersonnelDto(
    long PersonnelId,
    string? PersonnelName,
    int CompletedServiceCount,
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount);

public sealed record ReceptionReportServiceDto(
    long ServiceId,
    string ServiceName,
    int CompletedServiceCount,
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal AverageFinalAmount);

public sealed record ReceptionReportDebtDto(
    long ReceptionId,
    long ReceptionNumber,
    long CustomerId,
    string CustomerName,
    string? CustomerMobilePhone,
    DateTimeOffset ReceptionDate,
    ReceptionStatus Status,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount);

public sealed record ReceptionReportDebtTotalsDto(
    int ReceptionCount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount);

public sealed record ReceptionReportDiscountDto(
    long ReceptionId,
    long ReceptionNumber,
    long CustomerId,
    string CustomerName,
    string? CustomerMobilePhone,
    DateTimeOffset ReceptionDate,
    long ServiceId,
    string ServiceName,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount);

public sealed record ReceptionReportDiscountTotalsDto(
    int ServiceCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount);

public sealed record ReceptionReportMaterialDto(
    long MaterialId,
    long ServiceId,
    string ServiceName,
    string? MaterialName,
    string? BaseUnitName,
    string? BaseUnitSymbol,
    int UsageCount,
    decimal Quantity,
    decimal? TotalCost);

public sealed record ReceptionReportMaterialTotalsDto(
    int UsageCount,
    decimal Quantity,
    decimal TotalCost);

public sealed record ReceptionReportPagedDto<TItem, TTotals>(
    PagedList<TItem> Page,
    TTotals Totals);
