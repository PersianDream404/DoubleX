using Framwork.PagedList;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Application.Contract.DTOs;

public sealed record CreateExpenseRequestDto(
    string Category,
    decimal Amount,
    DateTimeOffset OccurredAt,
    string? Description,
    string IdempotencyKey);

public sealed record RegisterPersonnelSettlementRequestDto(
    long PersonnelId,
    PersonnelEarningType EarningType,
    decimal Amount,
    string PaymentMethod,
    string IdempotencyKey,
    string? Description);

public sealed record CancelExpenseRequestDto(string Reason);

public sealed record CalculateFixedSalaryRequestDto(
    long PersonnelId,
    DateOnly PeriodStart,
    DateOnly PeriodEnd);

public sealed class GetExpensesRequestDto : PagedParamData
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public string? Category { get; set; }
    public FinanceRecordStatus? Status { get; set; }
}

public sealed class GetPersonnelSettlementsRequestDto : PagedParamData
{
    public long? PersonnelId { get; set; }
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public PersonnelEarningType? EarningType { get; set; }
}

public sealed record ExpenseListItemDto(
    long Id,
    string Category,
    decimal Amount,
    DateTimeOffset OccurredAt,
    string? Description,
    FinanceRecordStatus Status,
    DateTimeOffset? CancelledAt,
    string? CancellationReason);

public sealed record PersonnelSettlementListItemDto(
    long Id,
    long PersonnelId,
    PersonnelEarningType EarningType,
    decimal Amount,
    DateTimeOffset PaidAt,
    string PaymentMethod,
    long CreatedBy,
    string? Description);

public sealed record PersonnelEarningsReportDto(
    long PersonnelId,
    decimal FixedSalaryAmount,
    decimal ServiceCommissionAmount,
    decimal TotalEarned,
    decimal SettledAmount,
    decimal RemainingAmount);

public sealed record ExpenseReportDto(
    decimal TotalAmount,
    IReadOnlyList<ExpenseCategoryTotalDto> Categories,
    Framwork.PagedList.PagedList<ExpenseListItemDto> Expenses);

public sealed record ExpenseCategoryTotalDto(string Category, decimal Amount);

public sealed record FinanceSummaryDto(
    decimal PersonnelObligations,
    decimal PersonnelSettlements,
    decimal PersonnelBalance,
    decimal RecordedExpenses,
    decimal CancelledExpenses);
