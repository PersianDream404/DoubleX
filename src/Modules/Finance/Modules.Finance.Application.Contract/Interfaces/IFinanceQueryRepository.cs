using Framwork.PagedList;
using Modules.Finance.Application.Contract.DTOs;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Application.Contract.Interfaces;

public interface IFinanceQueryRepository
{
    Task<bool> EarningExistsAsync(string idempotencyKey, long salonId, CancellationToken cancellationToken);
    Task<PagedList<ExpenseListItemDto>> GetExpensesAsync(
        long salonId,
        GetExpensesRequestDto request,
        CancellationToken cancellationToken);
    Task<ExpenseListItemDto?> GetExpenseByIdAsync(
        long salonId,
        long id,
        CancellationToken cancellationToken);
    Task<PagedList<PersonnelSettlementListItemDto>> GetSettlementsAsync(
        long salonId,
        GetPersonnelSettlementsRequestDto request,
        CancellationToken cancellationToken);
    Task<ExpenseReportDto> GetExpenseReportAsync(
        long salonId,
        GetExpensesRequestDto request,
        CancellationToken cancellationToken);
    Task<PersonnelEarningsReportDto> GetPersonnelEarningsReportAsync(
        long salonId,
        long personnelId,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken cancellationToken);
    Task<FinanceSummaryDto> GetSummaryAsync(
        long salonId,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken cancellationToken);
    Task<bool> SettlementExistsAsync(string idempotencyKey, long salonId, CancellationToken cancellationToken);
}
