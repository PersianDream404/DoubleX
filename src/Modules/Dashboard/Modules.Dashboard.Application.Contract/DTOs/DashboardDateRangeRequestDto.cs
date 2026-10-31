namespace Modules.Dashboard.Application.Contract.DTOs;

public enum DashboardPeriod
{
    Today = 1,
    Yesterday = 2,
    LastSevenDays = 3,
    CurrentMonth = 4,
    Custom = 5
}

public sealed class DashboardDateRangeRequestDto
{
    public DashboardPeriod Period { get; set; } = DashboardPeriod.Today;
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDateExclusive { get; set; }
}
