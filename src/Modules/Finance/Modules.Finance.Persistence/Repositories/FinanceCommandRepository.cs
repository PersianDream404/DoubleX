using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Modules.Finance.Domain.Entities;
using Modules.Finance.Domain.Interfaces;
using Modules.Finance.Persistence.Context;

namespace Modules.Finance.Persistence.Repositories;

public sealed class FinanceCommandRepository(FinanceWriteDbContext context)
    : IFinanceCommandRepository
{
    public async Task<bool> AddEarningAsync(
        PersonnelEarning earning,
        CancellationToken cancellationToken)
    {
        await context.PersonnelEarnings.AddAsync(earning, cancellationToken);
        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AddExpenseAsync(Expense expense, CancellationToken cancellationToken)
    {
        await context.Expenses.AddAsync(expense, cancellationToken);
        return await SaveChangesAsync(cancellationToken);
    }

    public Task<Expense?> GetExpenseAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Expenses.SingleOrDefaultAsync(
            expense => expense.Id == id && expense.SalonId == salonId,
            cancellationToken);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            context.ChangeTracker.Clear();
            return false;
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            context.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task<bool> AddSettlementWithinBalanceAsync(
        PersonnelSettlement settlement,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var earnings = await context.PersonnelEarnings
            .Where(earning =>
                earning.SalonId == settlement.SalonId &&
                earning.PersonnelId == settlement.PersonnelId &&
                earning.EarningType == settlement.EarningType &&
                earning.Status == FinanceRecordStatus.Active)
            .OrderBy(earning => earning.OccurredAt)
            .ThenBy(earning => earning.Id)
            .ToListAsync(cancellationToken);

        var earningIds = earnings.Select(earning => earning.Id).ToArray();
        var alreadyAllocated = await context.PersonnelSettlementItems
            .Where(item =>
                earningIds.Contains(item.PersonnelEarningId) &&
                item.Settlement.Status == FinanceRecordStatus.Active)
            .GroupBy(item => item.PersonnelEarningId)
            .Select(group => new
            {
                EarningId = group.Key,
                Amount = group.Sum(item => item.Amount)
            })
            .ToDictionaryAsync(item => item.EarningId, item => item.Amount, cancellationToken);

        var remaining = settlement.Amount;
        foreach (var earning in earnings)
        {
            var available = earning.Amount -
                (alreadyAllocated.TryGetValue(earning.Id, out var allocated) ? allocated : 0m);
            if (available <= 0)
            {
                continue;
            }

            var allocation = Math.Min(available, remaining);
            settlement.Allocate(earning.Id, allocation);
            remaining -= allocation;
            if (remaining == 0)
            {
                break;
            }
        }

        if (remaining > 0)
        {
            return false;
        }

        await context.PersonnelSettlements.AddAsync(settlement, cancellationToken);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            context.ChangeTracker.Clear();
            return false;
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            context.ChangeTracker.Clear();
            return false;
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}
