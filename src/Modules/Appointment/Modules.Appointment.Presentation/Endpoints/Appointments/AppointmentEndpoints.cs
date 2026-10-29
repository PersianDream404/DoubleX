using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.ChangeStatus;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.Create;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.Update;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetAll;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetAvailability;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetById;
using Modules.Appointment.Domain.Entities;
using Modules.Appointment.Presentation.Endpoints;
using SharedKernel.Interface;

namespace Modules.Appointment.Presentation.Endpoints.Appointments;

public sealed class CreateAppointmentEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost(AppointmentApiInfo.Prefix, async (
                    [FromBody] CreateAppointmentRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await commandBus.Send<CreateAppointmentCommand, long>(
                        new CreateAppointmentCommand(salonId, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { id = result.Value })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(AppointmentApiInfo.Tag);
    }
}

public sealed class GetAppointmentsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet(AppointmentApiInfo.Prefix, async (
                    [AsParameters] GetAppointmentsRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    request.SalonId = salonId;
                    var result = await queryBus.Send<
                        GetAppointmentsQuery,
                        PagedList<AppointmentListItemDto>>(
                        new GetAppointmentsQuery(request),
                        cancellationToken);
                    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(AppointmentApiInfo.Tag);
    }
}

public sealed class GetAppointmentByIdEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{AppointmentApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await queryBus.Send<GetAppointmentByIdQuery, AppointmentDetailsDto>(
                        new GetAppointmentByIdQuery(id, salonId),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(AppointmentApiInfo.Tag);
    }
}

public sealed class UpdateAppointmentEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut($"{AppointmentApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    [FromBody] UpdateAppointmentRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await commandBus.Send<UpdateAppointmentCommand, bool>(
                        new UpdateAppointmentCommand(id, salonId, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "اطلاعات نوبت به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(AppointmentApiInfo.Tag);
    }
}

public sealed class ChangeAppointmentStatusEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut($"{AppointmentApiInfo.Prefix}/{{id:long}}/status", async (
                    long id,
                    [FromBody] ChangeAppointmentStatusRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await commandBus.Send<ChangeAppointmentStatusCommand, bool>(
                        new ChangeAppointmentStatusCommand(id, salonId, request.Status),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "وضعیت نوبت به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(AppointmentApiInfo.Tag);
    }
}

public sealed class GetAppointmentAvailabilityEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{AppointmentApiInfo.Prefix}/availability", async (
                    [AsParameters] GetAppointmentAvailabilityRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await queryBus.Send<
                        GetAppointmentAvailabilityQuery,
                        AppointmentAvailabilityDto>(
                        new GetAppointmentAvailabilityQuery(salonId, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(AppointmentApiInfo.Tag);
    }
}
