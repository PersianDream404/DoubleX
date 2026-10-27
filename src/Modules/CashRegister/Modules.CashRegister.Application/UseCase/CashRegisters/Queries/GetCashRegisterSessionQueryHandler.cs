using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Queries;

public sealed class GetCashRegisterSessionQueryHandler(ICashRegisterQueryRepository repository)
    : IQueryHandler<GetCashRegisterSessionQuery, CashRegisterSessionDto>
{
    public async Task<Result<CashRegisterSessionDto>> Handle(
        GetCashRegisterSessionQuery query,
        CancellationToken cancellationToken)
    {
        var session = await repository.GetOpenSessionAsync(
            query.CashRegisterId, query.SalonId, cancellationToken);
        return session is null
            ? Result.NotFound("برای این صندوق جلسهٔ بازی وجود ندارد.")
            : Result.Success(session);
    }
}

public sealed class GetCashRegisterSessionQueryValidator : AbstractValidator<GetCashRegisterSessionQuery>
{
    public GetCashRegisterSessionQueryValidator()
    {
        RuleFor(query => query.CashRegisterId).GreaterThan(0);
        RuleFor(query => query.SalonId).GreaterThan(0);
    }
}
