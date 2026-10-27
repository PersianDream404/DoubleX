using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Queries;

public sealed class GetCashRegisterBalanceQueryHandler(ICashRegisterQueryRepository repository)
    : IQueryHandler<GetCashRegisterBalanceQuery, CashRegisterBalanceDto>
{
    public async Task<Result<CashRegisterBalanceDto>> Handle(
        GetCashRegisterBalanceQuery query,
        CancellationToken cancellationToken)
    {
        if (await repository.GetRegisterAsync(query.CashRegisterId, query.SalonId, cancellationToken) is null)
            return Result.NotFound("صندوق موردنظر پیدا نشد.");

        var balance = await repository.GetBalanceAsync(
            query.CashRegisterId, query.SalonId, cancellationToken);
        return balance is null
            ? Result.NotFound("برای این صندوق جلسهٔ بازی وجود ندارد.")
            : Result.Success(balance);
    }
}

public sealed class GetCashRegisterBalanceQueryValidator : AbstractValidator<GetCashRegisterBalanceQuery>
{
    public GetCashRegisterBalanceQueryValidator()
    {
        RuleFor(query => query.CashRegisterId).GreaterThan(0);
        RuleFor(query => query.SalonId).GreaterThan(0);
    }
}
