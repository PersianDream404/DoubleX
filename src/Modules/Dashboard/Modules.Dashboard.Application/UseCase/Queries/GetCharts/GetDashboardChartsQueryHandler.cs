using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;
using Modules.Dashboard.Application.Contract.Interfaces;
using Modules.Dashboard.Application.Contract.UseCase.Queries.GetCharts;
using Modules.Dashboard.Application.UseCase;

namespace Modules.Dashboard.Application.UseCase.Queries.GetCharts;

public sealed class GetDashboardChartsQueryHandler(IDashboardQueryRepository repository)
    : IQueryHandler<GetDashboardChartsQuery, DashboardChartsDto>
{
    public async Task<Result<DashboardChartsDto>> Handle(
        GetDashboardChartsQuery query,
        CancellationToken cancellationToken)
    {
        var range = DashboardDateRangeResolver.Resolve(
            query.Request,
            DateTimeOffset.UtcNow);
        var charts = await repository.GetChartsAsync(
            query.SalonId,
            range.From,
            range.ToExclusive,
            cancellationToken);
        return Result.Success(charts);
    }
}

public sealed class GetDashboardChartsQueryValidator
    : AbstractValidator<GetDashboardChartsQuery>
{
    public GetDashboardChartsQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Request).NotNull();
        RuleFor(query => query.Request.Period).IsInEnum();
        RuleFor(query => query.Request)
            .Must(request =>
                request.Period != DashboardPeriod.Custom ||
                request.FromDate.HasValue &&
                request.ToDateExclusive.HasValue &&
                request.ToDateExclusive.Value > request.FromDate.Value)
            .WithMessage("برای بازهٔ دلخواه، تاریخ شروع و پایان معتبر الزامی است.");
    }
}
