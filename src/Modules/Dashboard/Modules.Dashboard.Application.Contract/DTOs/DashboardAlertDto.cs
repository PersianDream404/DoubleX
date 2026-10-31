namespace Modules.Dashboard.Application.Contract.DTOs;

public enum DashboardAlertType
{
    PendingAppointments = 1,
    OutstandingDebts = 2,
    ExpiringPersonnelContracts = 3
}

public sealed record DashboardAlertDto(
    DashboardAlertType Type,
    string Title,
    int Count,
    decimal? Amount,
    IReadOnlyList<long> RelatedIds);
