using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Reception.Application.Contract.DTOs.Receptions.Update;
using Modules.Reception.Application.Contract.UseCase.Receptions.Commands.ChangeStatus;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Endpoints.Receptions.Write;

public sealed class ChangeReceptionStatusEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut($"{ReceptionApiInfo.Prefix}/{{id:long}}/status", async (
                    long id,
                    [FromBody] ChangeReceptionStatusRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    if (!ReceptionApiInfo.TryGetSalonId(httpContext, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    var result = await commandBus.Send<ChangeReceptionStatusCommand, bool>(
                        new ChangeReceptionStatusCommand(id, salonId, request.Status),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "وضعیت پذیرش به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ReceptionApiInfo.Tag);
    }
}
