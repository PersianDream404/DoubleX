using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.Interfaces;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetAll;

namespace Modules.Appointment.Application.UseCase.Appointments.Queries.GetAll;

public sealed class GetAppointmentsQueryHandler(IAppointmentQueryRepository repository)
    : IQueryHandler<GetAppointmentsQuery, PagedList<AppointmentListItemDto>>
{
    public async Task<Result<PagedList<AppointmentListItemDto>>> Handle(
        GetAppointmentsQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetAllAsync(query.Request, cancellationToken));
}

public sealed class GetAppointmentsQueryValidator : AbstractValidator<GetAppointmentsQuery>
{
    public GetAppointmentsQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
        {
            RuleFor(query => query.Request.SalonId)
                .NotNull()
                .GreaterThan(0);
            RuleFor(query => query.Request.CustomerId).GreaterThan(0).When(query =>
                query.Request.CustomerId.HasValue);
            RuleFor(query => query.Request.PersonnelId).GreaterThan(0).When(query =>
                query.Request.PersonnelId.HasValue);
            RuleFor(query => query.Request.ServiceId).GreaterThan(0).When(query =>
                query.Request.ServiceId.HasValue);
            RuleFor(query => query.Request.Status).IsInEnum().When(query =>
                query.Request.Status.HasValue);
            RuleFor(query => query.Request.PageNumber).GreaterThan(0);
            RuleFor(query => query.Request.PageSize).InclusiveBetween(1, 100);
            RuleFor(query => query.Request.To)
                .GreaterThan(query => query.Request.From)
                .When(query => query.Request.From.HasValue && query.Request.To.HasValue);
        });
    }
}
