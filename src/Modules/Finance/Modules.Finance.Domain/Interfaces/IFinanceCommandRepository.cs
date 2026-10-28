using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Domain.Interfaces;

public interface IFinanceCommandRepository
{
    Task<bool> AddEarningAsync(PersonnelEarning earning, CancellationToken cancellationToken);
    Task<bool> AddExpenseAsync(Expense expense, CancellationToken cancellationToken);
    Task<Expense?> GetExpenseAsync(long id, long salonId, CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    Task<bool> AddSettlementWithinBalanceAsync(
        PersonnelSettlement settlement,
        CancellationToken cancellationToken);
}
