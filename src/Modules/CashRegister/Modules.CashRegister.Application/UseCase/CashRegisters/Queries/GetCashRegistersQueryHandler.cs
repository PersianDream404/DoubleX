using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Queries;

public sealed class GetCashRegistersQueryHandler(ICashRegisterQueryRepository repository)
    : IQueryHandler<GetCashRegistersQuery, PagedList<CashRegisterDto>>
{
    public async Task<Result<PagedList<CashRegisterDto>>> Handle(
        GetCashRegistersQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Request.SalonId is null or <= 0)
            return Result.Invalid(new ValidationError("SalonId", "شناسهٔ سالن معتبر نیست."));
        return await repository.GetRegistersAsync(query.Request, cancellationToken);
    }
}

public sealed class GetCashRegistersQueryValidator : AbstractValidator<GetCashRegistersQuery>
{
    public GetCashRegistersQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
        {
            RuleFor(query => query.Request.SalonId).NotNull().GreaterThan(0);
            RuleFor(query => query.Request.PageNumber).GreaterThan(0);
            RuleFor(query => query.Request.PageSize).InclusiveBetween(1, 100);
            RuleFor(query => query.Request.Q).MaximumLength(150);
        });
    }
}
