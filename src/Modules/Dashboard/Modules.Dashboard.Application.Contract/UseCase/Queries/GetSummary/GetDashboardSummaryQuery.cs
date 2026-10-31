using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;

namespace Modules.Dashboard.Application.Contract.UseCase.Queries.GetSummary;

public sealed record GetDashboardSummaryQuery(
    long SalonId,
    DashboardDateRangeRequestDto Request) : IQuery<DashboardSummaryDto>;
