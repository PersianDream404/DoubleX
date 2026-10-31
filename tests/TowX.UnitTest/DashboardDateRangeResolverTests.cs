using FluentAssertions;
using Modules.Dashboard.Application.Contract.DTOs;
using Modules.Dashboard.Application.UseCase;

namespace TowX.UnitTest;

public sealed class DashboardDateRangeResolverTests
{
    [Theory]
    [InlineData(DashboardPeriod.Today, 2026, 10, 31, 0, 0)]
    [InlineData(DashboardPeriod.Yesterday, 2026, 10, 30, 0, 0)]
    [InlineData(DashboardPeriod.LastSevenDays, 2026, 10, 25, 0, 0)]
    [InlineData(DashboardPeriod.CurrentMonth, 2026, 10, 1, 0, 0)]
    public void Resolve_UsesUtcBoundariesForPresetPeriods(
        DashboardPeriod period,
        int year,
        int month,
        int day,
        int hour,
        int minute)
    {
        var now = new DateTimeOffset(2026, 10, 31, 16, 40, 0, TimeSpan.FromHours(3.5));

        var range = DashboardDateRangeResolver.Resolve(
            new DashboardDateRangeRequestDto { Period = period },
            now);

        range.From.Should().Be(new DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.Zero));
        if (period != DashboardPeriod.Yesterday)
        {
            range.ToExclusive.Should().Be(now.ToUniversalTime());
        }
        else
        {
            range.ToExclusive.Should().Be(new DateTimeOffset(2026, 10, 31, 0, 0, 0, TimeSpan.Zero));
        }
    }

    [Fact]
    public void Resolve_NormalizesCustomRangeToUtc()
    {
        var request = new DashboardDateRangeRequestDto
        {
            Period = DashboardPeriod.Custom,
            FromDate = new DateTimeOffset(2026, 10, 1, 8, 0, 0, TimeSpan.FromHours(3.5)),
            ToDateExclusive = new DateTimeOffset(2026, 10, 10, 8, 0, 0, TimeSpan.FromHours(3.5))
        };

        var range = DashboardDateRangeResolver.Resolve(
            request,
            DateTimeOffset.UtcNow);

        range.From.Should().Be(request.FromDate.Value.ToUniversalTime());
        range.ToExclusive.Should().Be(request.ToDateExclusive.Value.ToUniversalTime());
    }

    [Fact]
    public void Resolve_RejectsInvalidCustomRange()
    {
        var request = new DashboardDateRangeRequestDto
        {
            Period = DashboardPeriod.Custom,
            FromDate = new DateTimeOffset(2026, 10, 10, 0, 0, 0, TimeSpan.Zero),
            ToDateExclusive = new DateTimeOffset(2026, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };

        var act = () => DashboardDateRangeResolver.Resolve(request, DateTimeOffset.UtcNow);

        act.Should().Throw<ArgumentException>();
    }
}
