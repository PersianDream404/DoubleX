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

namespace Modules.Appointment.Presentation.Endpoints.Appointments.Write;

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
