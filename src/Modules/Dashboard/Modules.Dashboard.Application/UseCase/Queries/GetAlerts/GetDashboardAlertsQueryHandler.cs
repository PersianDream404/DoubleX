using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;
using Modules.Dashboard.Application.Contract.Interfaces;
using Modules.Dashboard.Application.Contract.UseCase.Queries.GetAlerts;

namespace Modules.Dashboard.Application.UseCase.Queries.GetAlerts;

public sealed class GetDashboardAlertsQueryHandler(IDashboardQueryRepository repository)
    : IQueryHandler<GetDashboardAlertsQuery, IReadOnlyList<DashboardAlertDto>>
{
    public async Task<Result<IReadOnlyList<DashboardAlertDto>>> Handle(
        GetDashboardAlertsQuery query,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var alerts = await repository.GetAlertsAsync(
            query.SalonId,
            today,
            cancellationToken);
        return Result.Success(alerts);
    }
}

public sealed class GetDashboardAlertsQueryValidator
    : AbstractValidator<GetDashboardAlertsQuery>
{
    public GetDashboardAlertsQueryValidator() =>
        RuleFor(query => query.SalonId).GreaterThan(0);
}
