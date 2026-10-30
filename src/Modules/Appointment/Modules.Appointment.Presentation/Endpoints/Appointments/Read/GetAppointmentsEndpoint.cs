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

namespace Modules.Appointment.Presentation.Endpoints.Appointments.Read;

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
