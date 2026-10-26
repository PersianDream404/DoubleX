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
                    [FromQuery] long salonId,
                    [FromQuery] long serviceId,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<RemoveServicePersonnelCommand, bool>(
                        new RemoveServicePersonnelCommand(salonId, serviceId, id),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "پرسنل از خدمت حذف شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
