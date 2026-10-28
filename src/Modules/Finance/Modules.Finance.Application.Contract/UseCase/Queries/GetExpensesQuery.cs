using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Finance.Application.Contract.DTOs;

namespace Modules.Finance.Application.Contract.UseCase.Queries;

public sealed record GetExpensesQuery(long SalonId, GetExpensesRequestDto Request)
    : IQuery<PagedList<ExpenseListItemDto>>;
