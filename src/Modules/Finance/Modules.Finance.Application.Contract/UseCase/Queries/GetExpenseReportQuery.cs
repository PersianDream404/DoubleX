using Framwork.Bus.Query;
using Modules.Finance.Application.Contract.DTOs;

namespace Modules.Finance.Application.Contract.UseCase.Queries;

public sealed record GetExpenseReportQuery(long SalonId, GetExpensesRequestDto Request)
    : IQuery<ExpenseReportDto>;
