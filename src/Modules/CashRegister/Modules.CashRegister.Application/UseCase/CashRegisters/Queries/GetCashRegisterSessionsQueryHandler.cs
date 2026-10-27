using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Queries;

public sealed class GetCashRegisterSessionsQueryHandler(ICashRegisterQueryRepository repository)
    : IQueryHandler<GetCashRegisterSessionsQuery, PagedList<CashRegisterSessionDto>>
{
    public async Task<Result<PagedList<CashRegisterSessionDto>>> Handle(
        GetCashRegisterSessionsQuery query,
        CancellationToken cancellationToken)
    {
        if (await repository.GetRegisterAsync(query.CashRegisterId, query.SalonId, cancellationToken) is null)
            return Result.NotFound("صندوق موردنظر پیدا نشد.");

        return await repository.GetSessionsAsync(
            query.CashRegisterId,
            query.SalonId,
            query.PageNumber,
            query.PageSize,
            cancellationToken);
    }
}

public sealed class GetCashRegisterSessionsQueryValidator : AbstractValidator<GetCashRegisterSessionsQuery>
{
    public GetCashRegisterSessionsQueryValidator()
    {
        RuleFor(query => query.CashRegisterId).GreaterThan(0);
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.PageNumber).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
