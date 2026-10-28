using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Finance.Application.Contract.DTOs;
using Modules.Finance.Application.Contract.Interfaces;
using Modules.Finance.Application.Contract.UseCase.Queries;
using Modules.Finance.Domain.Entities;

namespace Modules.Finance.Application.UseCase.Queries;

public sealed class GetExpenseByIdQueryHandler(IFinanceQueryRepository repository)
    : IQueryHandler<GetExpenseByIdQuery, ExpenseListItemDto>
{
    public async Task<Result<ExpenseListItemDto>> Handle(
        GetExpenseByIdQuery query,
        CancellationToken cancellationToken)
    {
        var expense = await repository.GetExpenseByIdAsync(
            query.SalonId,
            query.Id,
            cancellationToken);
        return expense is null
            ? Result.NotFound("هزینهٔ موردنظر پیدا نشد.")
            : Result.Success(expense);
    }
}

public sealed class GetExpenseByIdQueryValidator : AbstractValidator<GetExpenseByIdQuery>
{
    public GetExpenseByIdQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Id).GreaterThan(0);
    }
}

public sealed class GetExpensesQueryHandler(IFinanceQueryRepository repository)
    : IQueryHandler<GetExpensesQuery, PagedList<ExpenseListItemDto>>
{
    public async Task<Result<PagedList<ExpenseListItemDto>>> Handle(
        GetExpensesQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetExpensesAsync(
            query.SalonId,
            query.Request,
            cancellationToken));
}

public sealed class GetPersonnelSettlementsQueryHandler(IFinanceQueryRepository repository)
    : IQueryHandler<GetPersonnelSettlementsQuery, PagedList<PersonnelSettlementListItemDto>>
{
    public async Task<Result<PagedList<PersonnelSettlementListItemDto>>> Handle(
        GetPersonnelSettlementsQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetSettlementsAsync(
            query.SalonId,
            query.Request,
            cancellationToken));
}

public sealed class GetPersonnelSettlementsQueryValidator
    : AbstractValidator<GetPersonnelSettlementsQuery>
{
    public GetPersonnelSettlementsQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Request).NotNull();
        RuleFor(query => query.Request.PageNumber).GreaterThan(0);
        RuleFor(query => query.Request.PageSize).InclusiveBetween(1, 200);
        RuleFor(query => query.Request.PersonnelId)
            .GreaterThan(0)
            .When(query => query.Request.PersonnelId.HasValue);
        RuleFor(query => query.Request.EarningType)
            .IsInEnum()
            .When(query => query.Request.EarningType.HasValue);
        RuleFor(query => query.Request.From)
            .LessThanOrEqualTo(query => query.Request.To)
            .When(query => query.Request.From.HasValue && query.Request.To.HasValue);
    }
}

public sealed class GetExpensesQueryValidator : AbstractValidator<GetExpensesQuery>
{
    public GetExpensesQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Request).NotNull();
        RuleFor(query => query.Request.PageNumber).GreaterThan(0);
        RuleFor(query => query.Request.PageSize).InclusiveBetween(1, 200);
        RuleFor(query => query.Request.From)
            .LessThanOrEqualTo(query => query.Request.To)
            .When(query => query.Request.From.HasValue && query.Request.To.HasValue);
    }
}

public sealed class GetExpenseReportQueryHandler(IFinanceQueryRepository repository)
    : IQueryHandler<GetExpenseReportQuery, ExpenseReportDto>
{
    public async Task<Result<ExpenseReportDto>> Handle(
        GetExpenseReportQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetExpenseReportAsync(
            query.SalonId,
            query.Request,
            cancellationToken));
}

public sealed class GetExpenseReportQueryValidator : AbstractValidator<GetExpenseReportQuery>
{
    public GetExpenseReportQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Request).NotNull();
        RuleFor(query => query.Request.PageNumber).GreaterThan(0);
        RuleFor(query => query.Request.PageSize).InclusiveBetween(1, 200);
        RuleFor(query => query.Request.From)
            .LessThanOrEqualTo(query => query.Request.To)
            .When(query => query.Request.From.HasValue && query.Request.To.HasValue);
    }
}

public sealed class GetPersonnelEarningsReportQueryHandler(IFinanceQueryRepository repository)
    : IQueryHandler<GetPersonnelEarningsReportQuery, PersonnelEarningsReportDto>
{
    public async Task<Result<PersonnelEarningsReportDto>> Handle(
        GetPersonnelEarningsReportQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetPersonnelEarningsReportAsync(
            query.SalonId,
            query.PersonnelId,
            query.From,
            query.To,
            cancellationToken));
}

public sealed class GetPersonnelEarningsReportQueryValidator
    : AbstractValidator<GetPersonnelEarningsReportQuery>
{
    public GetPersonnelEarningsReportQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.PersonnelId).GreaterThan(0);
        RuleFor(query => query.From)
            .LessThanOrEqualTo(query => query.To)
            .When(query => query.From.HasValue && query.To.HasValue);
    }
}

public sealed class GetFinanceSummaryQueryHandler(IFinanceQueryRepository repository)
    : IQueryHandler<GetFinanceSummaryQuery, FinanceSummaryDto>
{
    public async Task<Result<FinanceSummaryDto>> Handle(
        GetFinanceSummaryQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetSummaryAsync(
            query.SalonId,
            query.From,
            query.To,
            cancellationToken));
}

public sealed class GetFinanceSummaryQueryValidator : AbstractValidator<GetFinanceSummaryQuery>
{
    public GetFinanceSummaryQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.From)
            .LessThanOrEqualTo(query => query.To)
            .When(query => query.From.HasValue && query.To.HasValue);
    }
}
