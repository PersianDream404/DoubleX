using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Queries;

public sealed class GetCashRegisterTransactionsQueryHandler(ICashRegisterQueryRepository repository)
    : IQueryHandler<GetCashRegisterTransactionsQuery, PagedList<CashRegisterTransactionDto>>
{
    public async Task<Result<PagedList<CashRegisterTransactionDto>>> Handle(
        GetCashRegisterTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Request.SalonId is null or <= 0)
            return Result.Invalid(new ValidationError("SalonId", "شناسهٔ سالن معتبر نیست."));
        if (await repository.GetRegisterAsync(query.CashRegisterId, query.Request.SalonId.Value, cancellationToken) is null)
            return Result.NotFound("صندوق موردنظر پیدا نشد.");

        return await repository.GetTransactionsAsync(query.CashRegisterId, query.Request, cancellationToken);
    }
}

public sealed class GetCashRegisterTransactionsQueryValidator
    : AbstractValidator<GetCashRegisterTransactionsQuery>
{
    public GetCashRegisterTransactionsQueryValidator()
    {
        RuleFor(query => query.CashRegisterId).GreaterThan(0);
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
        {
            RuleFor(query => query.Request.SalonId).NotNull().GreaterThan(0);
            RuleFor(query => query.Request.PageNumber).GreaterThan(0);
            RuleFor(query => query.Request.PageSize).InclusiveBetween(1, 100);
            RuleFor(query => query.Request.From)
                .LessThanOrEqualTo(query => query.Request.To)
                .When(query => query.Request.From.HasValue && query.Request.To.HasValue);
        });
    }
}
