using Modules.Dashboard.Application.Contract.DTOs;

namespace Modules.Dashboard.Application.UseCase;

public sealed record ResolvedDashboardDateRange(
    DateTimeOffset From,
    DateTimeOffset ToExclusive);

public static class DashboardDateRangeResolver
{
    public static ResolvedDashboardDateRange Resolve(
        DashboardDateRangeRequestDto request,
        DateTimeOffset utcNow)
    {
        var today = DateOnly.FromDateTime(utcNow.UtcDateTime);
        var now = utcNow.ToUniversalTime();

        return request.Period switch
        {
            DashboardPeriod.Today => new(
                StartOfDay(today),
                now),
            DashboardPeriod.Yesterday => new(
                StartOfDay(today.AddDays(-1)),
                StartOfDay(today)),
            DashboardPeriod.LastSevenDays => new(
                StartOfDay(today.AddDays(-6)),
                now),
            DashboardPeriod.CurrentMonth => new(
                StartOfDay(new DateOnly(today.Year, today.Month, 1)),
                now),
            DashboardPeriod.Custom when
                request.FromDate.HasValue &&
                request.ToDateExclusive.HasValue &&
                request.ToDateExclusive.Value > request.FromDate.Value =>
                new(
                    request.FromDate.Value.ToUniversalTime(),
                    request.ToDateExclusive.Value.ToUniversalTime()),
            DashboardPeriod.Custom =>
                throw new ArgumentException(
                    "برای بازهٔ دلخواه، تاریخ شروع و پایان معتبر الزامی است.",
                    nameof(request)),
            _ => throw new ArgumentOutOfRangeException(
                nameof(request.Period),
                request.Period,
                "بازهٔ زمانی داشبورد نامعتبر است.")
        };
    }

    private static DateTimeOffset StartOfDay(DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}
