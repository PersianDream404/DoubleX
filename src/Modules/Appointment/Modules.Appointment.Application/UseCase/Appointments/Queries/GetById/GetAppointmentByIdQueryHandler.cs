using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.Interfaces;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetById;

namespace Modules.Appointment.Application.UseCase.Appointments.Queries.GetById;

public sealed class GetAppointmentByIdQueryHandler(IAppointmentQueryRepository repository)
    : IQueryHandler<GetAppointmentByIdQuery, AppointmentDetailsDto>
{
    public async Task<Result<AppointmentDetailsDto>> Handle(
        GetAppointmentByIdQuery query,
        CancellationToken cancellationToken)
    {
        var appointment = await repository.GetByIdAsync(
            query.Id, query.SalonId, cancellationToken);
        return appointment is null
            ? Result.NotFound("نوبت موردنظر پیدا نشد.")
            : Result.Success(appointment);
    }
}

public sealed class GetAppointmentByIdQueryValidator : AbstractValidator<GetAppointmentByIdQuery>
{
    public GetAppointmentByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
        RuleFor(query => query.SalonId).GreaterThan(0);
    }
}
