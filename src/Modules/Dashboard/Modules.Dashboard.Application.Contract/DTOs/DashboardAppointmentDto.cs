namespace Modules.Dashboard.Application.Contract.DTOs;

public sealed record DashboardAppointmentDto(
    long Id,
    long? ReceptionId,
    string CustomerName,
    string ServiceName,
    string PersonnelName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    decimal FinalAmount,
    string Status);

public sealed record DashboardTodayAppointmentsDto(
    IReadOnlyList<DashboardAppointmentDto> Appointments,
    int TotalCount);
