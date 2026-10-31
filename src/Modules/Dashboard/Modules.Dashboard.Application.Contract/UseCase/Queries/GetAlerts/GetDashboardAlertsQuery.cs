using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;

namespace Modules.Dashboard.Application.Contract.UseCase.Queries.GetAlerts;

public sealed record GetDashboardAlertsQuery(long SalonId)
    : IQuery<IReadOnlyList<DashboardAlertDto>>;
