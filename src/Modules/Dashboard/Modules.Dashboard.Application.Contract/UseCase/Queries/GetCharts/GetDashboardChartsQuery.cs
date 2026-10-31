using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;

namespace Modules.Dashboard.Application.Contract.UseCase.Queries.GetCharts;

public sealed record GetDashboardChartsQuery(
    long SalonId,
    DashboardDateRangeRequestDto Request) : IQuery<DashboardChartsDto>;
