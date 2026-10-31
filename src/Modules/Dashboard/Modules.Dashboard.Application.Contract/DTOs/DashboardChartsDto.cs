namespace Modules.Dashboard.Application.Contract.DTOs;

public sealed record DashboardFinancialTrendDto(
    DateOnly Date,
    int ReceptionCount,
    decimal ServiceSubtotal,
    decimal DiscountAmount,
    decimal ServiceFinalAmount,
    decimal PaidAmount);

public sealed record DashboardPopularServiceDto(
    long ServiceId,
    string ServiceName,
    int CompletedServiceCount,
    decimal ServiceAmount);

public sealed record DashboardPersonnelPerformanceDto(
    long PersonnelId,
    string? PersonnelName,
    int CompletedServiceCount,
    decimal ServiceAmount);

public sealed record DashboardChartsDto(
    IReadOnlyList<DashboardFinancialTrendDto> FinancialTrend,
    decimal RecordedExpenses,
    decimal CancelledExpenses,
    IReadOnlyList<DashboardPopularServiceDto> PopularServices,
    IReadOnlyList<DashboardPersonnelPerformanceDto> PersonnelPerformance);
