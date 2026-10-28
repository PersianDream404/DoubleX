using Framwork.PagedList;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Modules.Finance.Application.Contract.DTOs;
using Modules.Finance.Application.Contract.Interfaces;
using Modules.Finance.Domain.Entities;
using Modules.Finance.Persistence.Context;
using Modules.Finance.Persistence.Mapper;

namespace Modules.Finance.Persistence.Repositories;

public sealed class FinanceQueryRepository(FinanceReadDbContext context)
    : IFinanceQueryRepository
{
    public Task<bool> EarningExistsAsync(
        string idempotencyKey,
        long salonId,
        CancellationToken cancellationToken) =>
        context.PersonnelEarnings.AnyAsync(
            earning => earning.SalonId == salonId && earning.IdempotencyKey == idempotencyKey,
            cancellationToken);

    public Task<bool> SettlementExistsAsync(
        string idempotencyKey,
        long salonId,
        CancellationToken cancellationToken) =>
        context.PersonnelSettlements.AnyAsync(
            settlement => settlement.SalonId == salonId &&
                          settlement.IdempotencyKey == idempotencyKey,
            cancellationToken);

    public async Task<PagedList<ExpenseListItemDto>> GetExpensesAsync(
        long salonId,
        GetExpensesRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = BuildExpenseQuery(salonId, request);
        return await query
            .OrderByDescending(expense => expense.OccurredAt)
            .ThenByDescending(expense => expense.Id)
            .Select(FinanceMapper.ToExpenseListItemDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public Task<ExpenseListItemDto?> GetExpenseByIdAsync(
        long salonId,
        long id,
        CancellationToken cancellationToken) =>
        context.Expenses
            .AsNoTracking()
            .Where(expense => expense.SalonId == salonId && expense.Id == id)
            .Select(FinanceMapper.ToExpenseListItemDto())
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<PagedList<PersonnelSettlementListItemDto>> GetSettlementsAsync(
        long salonId,
        GetPersonnelSettlementsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.PersonnelSettlements
            .AsNoTracking()
            .Where(settlement =>
                settlement.SalonId == salonId &&
                settlement.Status == FinanceRecordStatus.Active);
        if (request.PersonnelId.HasValue)
        {
            query = query.Where(settlement =>
                settlement.PersonnelId == request.PersonnelId.Value);
        }
        if (request.EarningType.HasValue)
        {
            query = query.Where(settlement =>
                settlement.EarningType == request.EarningType.Value);
        }
        if (request.From.HasValue)
        {
            query = query.Where(settlement => settlement.PaidAt >= request.From.Value);
        }
        if (request.To.HasValue)
        {
            query = query.Where(settlement => settlement.PaidAt <= request.To.Value);
        }

        return await query
            .OrderByDescending(settlement => settlement.PaidAt)
            .ThenByDescending(settlement => settlement.Id)
            .Select(FinanceMapper.ToPersonnelSettlementListItemDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public async Task<ExpenseReportDto> GetExpenseReportAsync(
        long salonId,
        GetExpensesRequestDto request,
        CancellationToken cancellationToken)
    {
        var expenses = BuildExpenseQuery(salonId, request)
            .Where(expense => expense.Status == FinanceRecordStatus.Active);
        var total = await expenses.SumAsync(expense => (decimal?)expense.Amount, cancellationToken) ?? 0m;
        var categories = await expenses
            .GroupBy(expense => expense.Category)
            .Select(FinanceMapper.ToExpenseCategoryTotalDto())
            .OrderBy(item => item.Category)
            .ToListAsync(cancellationToken);
        var list = await GetExpensesAsync(salonId, request, cancellationToken);
        return new ExpenseReportDto(total, categories, list);
    }

    public async Task<PersonnelEarningsReportDto> GetPersonnelEarningsReportAsync(
        long salonId,
        long personnelId,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken cancellationToken)
    {
        var earnings = BuildEarningsQuery(salonId, from, to)
            .Where(earning => earning.PersonnelId == personnelId);
        var fixedSalary = await earnings
            .Where(earning => earning.EarningType == PersonnelEarningType.FixedSalary)
            .SumAsync(earning => (decimal?)earning.Amount, cancellationToken) ?? 0m;
        var commission = await earnings
            .Where(earning => earning.EarningType == PersonnelEarningType.ServiceCommission)
            .SumAsync(earning => (decimal?)earning.Amount, cancellationToken) ?? 0m;
        var settled = await context.PersonnelSettlementItems
            .Where(item =>
                item.Settlement.SalonId == salonId &&
                item.Settlement.PersonnelId == personnelId &&
                item.Settlement.Status == FinanceRecordStatus.Active &&
                item.Earning.SalonId == salonId &&
                item.Earning.PersonnelId == personnelId &&
                item.Earning.Status == FinanceRecordStatus.Active &&
                (!from.HasValue || item.Earning.OccurredAt >= from.Value) &&
                (!to.HasValue || item.Earning.OccurredAt <= to.Value))
            .SumAsync(item => (decimal?)item.Amount, cancellationToken) ?? 0m;
        var earned = fixedSalary + commission;
        return new PersonnelEarningsReportDto(
            personnelId,
            fixedSalary,
            commission,
            earned,
            settled,
            earned - settled);
    }

    public async Task<FinanceSummaryDto> GetSummaryAsync(
        long salonId,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken cancellationToken)
    {
        var earnings = BuildEarningsQuery(salonId, from, to);
        var obligations = await earnings.SumAsync(
            earning => (decimal?)earning.Amount,
            cancellationToken) ?? 0m;
        var settlements = await context.PersonnelSettlementItems
            .Where(item =>
                item.Settlement.SalonId == salonId &&
                item.Settlement.Status == FinanceRecordStatus.Active &&
                item.Earning.SalonId == salonId &&
                item.Earning.Status == FinanceRecordStatus.Active &&
                (!from.HasValue || item.Earning.OccurredAt >= from.Value) &&
                (!to.HasValue || item.Earning.OccurredAt <= to.Value))
            .SumAsync(item => (decimal?)item.Amount, cancellationToken) ?? 0m;

        var expenses = context.Expenses.Where(expense =>
            expense.SalonId == salonId &&
            (!from.HasValue || expense.OccurredAt >= from.Value) &&
            (!to.HasValue || expense.OccurredAt <= to.Value));
        var recordedExpenses = await expenses
            .Where(expense => expense.Status == FinanceRecordStatus.Active)
            .SumAsync(expense => (decimal?)expense.Amount, cancellationToken) ?? 0m;
        var cancelledExpenses = await expenses
            .Where(expense => expense.Status == FinanceRecordStatus.Cancelled)
            .SumAsync(expense => (decimal?)expense.Amount, cancellationToken) ?? 0m;

        return new FinanceSummaryDto(
            obligations,
            settlements,
            obligations - settlements,
            recordedExpenses,
            cancelledExpenses);
    }

    private IQueryable<Expense> BuildExpenseQuery(long salonId, GetExpensesRequestDto request)
    {
        var query = context.Expenses
            .AsNoTracking()
            .Where(expense => expense.SalonId == salonId);
        if (request.From.HasValue)
        {
            query = query.Where(expense => expense.OccurredAt >= request.From.Value);
        }
        if (request.To.HasValue)
        {
            query = query.Where(expense => expense.OccurredAt <= request.To.Value);
        }
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var category = request.Category.Trim();
            query = query.Where(expense => expense.Category == category);
        }
        if (request.Status.HasValue)
        {
            query = query.Where(expense => expense.Status == request.Status.Value);
        }
        return query;
    }

    private IQueryable<PersonnelEarning> BuildEarningsQuery(
        long salonId,
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        var query = context.PersonnelEarnings
            .AsNoTracking()
            .Where(earning =>
                earning.SalonId == salonId &&
                earning.Status == FinanceRecordStatus.Active);
        if (from.HasValue)
        {
            query = query.Where(earning => earning.OccurredAt >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(earning => earning.OccurredAt <= to.Value);
        }
        return query;
    }
}
