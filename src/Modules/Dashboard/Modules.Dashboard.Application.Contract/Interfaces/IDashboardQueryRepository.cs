using Modules.Dashboard.Application.Contract.DTOs;

namespace Modules.Dashboard.Application.Contract.Interfaces;

public interface IDashboardQueryRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        long salonId,
        DateTimeOffset from,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken);

    Task<DashboardTodayAppointmentsDto> GetTodayAppointmentsAsync(
        long salonId,
        DateTimeOffset from,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DashboardAlertDto>> GetAlertsAsync(
        long salonId,
        DateOnly today,
        CancellationToken cancellationToken);

    Task<DashboardChartsDto> GetChartsAsync(
        long salonId,
        DateTimeOffset from,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken);
}
