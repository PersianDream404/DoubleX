using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Commands.Remove;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePersonnels.Remove;

public sealed class RemoveServicePersonnelEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapDelete($"{ServiceApiInfo.Prefix}/Personnels/{{id:long}}", async (
                    long id,
                    [FromQuery] long serviceId,
                    [FromServices] ICommandBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<RemoveServicePersonnelCommand, bool>(
                        new RemoveServicePersonnelCommand(salonId, serviceId, id),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "پرسنل از خدمت حذف شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}
