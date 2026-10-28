using Framwork.Bus.Query;
using Modules.Finance.Application.Contract.DTOs;

namespace Modules.Finance.Application.Contract.UseCase.Queries;

public sealed record GetExpenseByIdQuery(long SalonId, long Id) : IQuery<ExpenseListItemDto>;
