using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;

namespace Modules.Dashboard.Application.Contract.UseCase.Queries.GetTodayAppointments;

public sealed record GetDashboardTodayAppointmentsQuery(long SalonId)
    : IQuery<DashboardTodayAppointmentsDto>;
