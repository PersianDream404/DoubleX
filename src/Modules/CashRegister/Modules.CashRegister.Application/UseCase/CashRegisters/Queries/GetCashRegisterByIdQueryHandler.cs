using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Queries;

public sealed class GetCashRegisterByIdQueryHandler(ICashRegisterQueryRepository repository)
    : IQueryHandler<GetCashRegisterByIdQuery, CashRegisterDto>
{
    public async Task<Result<CashRegisterDto>> Handle(
        GetCashRegisterByIdQuery query,
        CancellationToken cancellationToken)
    {
        var register = await repository.GetRegisterAsync(query.Id, query.SalonId, cancellationToken);
        return register is null ? Result.NotFound("صندوق موردنظر پیدا نشد.") : Result.Success(register);
    }
}

public sealed class GetCashRegisterByIdQueryValidator : AbstractValidator<GetCashRegisterByIdQuery>
{
    public GetCashRegisterByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
        RuleFor(query => query.SalonId).GreaterThan(0);
    }
}
