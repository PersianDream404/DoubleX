using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;
using Modules.Dashboard.Application.Contract.Interfaces;
using Modules.Dashboard.Application.Contract.UseCase.Queries.GetSummary;
using Modules.Dashboard.Application.UseCase;

namespace Modules.Dashboard.Application.UseCase.Queries.GetSummary;

public sealed class GetDashboardSummaryQueryHandler(IDashboardQueryRepository repository)
    : IQueryHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<Result<DashboardSummaryDto>> Handle(
        GetDashboardSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var range = DashboardDateRangeResolver.Resolve(
            query.Request,
            DateTimeOffset.UtcNow);
        var summary = await repository.GetSummaryAsync(
            query.SalonId,
            range.From,
            range.ToExclusive,
            cancellationToken);
        return Result.Success(summary);
    }
}

public sealed class GetDashboardSummaryQueryValidator
    : AbstractValidator<GetDashboardSummaryQuery>
{
    public GetDashboardSummaryQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Request).NotNull();
        RuleFor(query => query.Request.Period).IsInEnum();
        RuleFor(query => query.Request)
            .Must(HasValidRange)
            .WithMessage("برای بازهٔ دلخواه، تاریخ شروع و پایان معتبر الزامی است.");
    }

    private static bool HasValidRange(DashboardDateRangeRequestDto request) =>
        request.Period != DashboardPeriod.Custom ||
        request.FromDate.HasValue &&
        request.ToDateExclusive.HasValue &&
        request.ToDateExclusive.Value > request.FromDate.Value;
}
