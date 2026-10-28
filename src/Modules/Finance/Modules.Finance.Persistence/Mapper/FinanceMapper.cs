using System.Linq.Expressions;
using Modules.Finance.Application.Contract.DTOs;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Persistence.Mapper;

public static class FinanceMapper
{
    public static Expression<Func<Expense, ExpenseListItemDto>> ToExpenseListItemDto() =>
        expense => new ExpenseListItemDto(
            expense.Id,
            expense.Category,
            expense.Amount,
            expense.OccurredAt,
            expense.Description,
            expense.Status,
            expense.CancelledAt,
            expense.CancellationReason);

    public static Expression<Func<PersonnelSettlement, PersonnelSettlementListItemDto>>
        ToPersonnelSettlementListItemDto() =>
        settlement => new PersonnelSettlementListItemDto(
            settlement.Id,
            settlement.PersonnelId,
            settlement.EarningType,
            settlement.Amount,
            settlement.PaidAt,
            settlement.PaymentMethod,
            settlement.CreatedBy,
            settlement.Description);

    public static Expression<Func<IGrouping<string, Expense>, ExpenseCategoryTotalDto>>
        ToExpenseCategoryTotalDto() =>
        group => new ExpenseCategoryTotalDto(
            group.Key,
            group.Sum(expense => expense.Amount));
}
