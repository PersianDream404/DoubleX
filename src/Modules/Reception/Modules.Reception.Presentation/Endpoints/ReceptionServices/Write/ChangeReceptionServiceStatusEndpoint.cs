using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Reception.Application.Contract.DTOs.ReceptionServices.Add;
using Modules.Reception.Application.Contract.UseCase.ReceptionServices.Commands.ChangeStatus;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Endpoints.ReceptionServices.Write;

public sealed class ChangeReceptionServiceStatusEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut(
                $"{ReceptionApiInfo.Prefix}/{{receptionId:long}}/services/{{serviceId:long}}/status",
                async (
                    long receptionId,
                    long serviceId,
                    [FromBody] ChangeReceptionServiceStatusRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    if (!ReceptionApiInfo.TryGetSalonId(httpContext, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    var result = await commandBus.Send<ChangeReceptionServiceStatusCommand, bool>(
                        new ChangeReceptionServiceStatusCommand(
                            receptionId,
                            serviceId,
                            request with { SalonId = salonId }),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "وضعیت خدمت به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ReceptionApiInfo.Tag);
    }
}
