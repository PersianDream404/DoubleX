using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Dashboard.Application.Contract.DTOs;
using Modules.Dashboard.Application.Contract.Interfaces;
using Modules.Dashboard.Application.Contract.UseCase.Queries.GetTodayAppointments;

namespace Modules.Dashboard.Application.UseCase.Queries.GetTodayAppointments;

public sealed class GetDashboardTodayAppointmentsQueryHandler(
    IDashboardQueryRepository repository)
    : IQueryHandler<
        GetDashboardTodayAppointmentsQuery,
        DashboardTodayAppointmentsDto>
{
    public async Task<Result<DashboardTodayAppointmentsDto>> Handle(
        GetDashboardTodayAppointmentsQuery query,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = new DateTimeOffset(today.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var toExclusive = new DateTimeOffset(
            today.AddDays(1).ToDateTime(TimeOnly.MinValue),
            TimeSpan.Zero);
        var appointments = await repository.GetTodayAppointmentsAsync(
            query.SalonId,
            from,
            toExclusive,
            cancellationToken);
        return Result.Success(appointments);
    }
}

public sealed class GetDashboardTodayAppointmentsQueryValidator
    : AbstractValidator<GetDashboardTodayAppointmentsQuery>
{
    public GetDashboardTodayAppointmentsQueryValidator() =>
        RuleFor(query => query.SalonId).GreaterThan(0);
}
