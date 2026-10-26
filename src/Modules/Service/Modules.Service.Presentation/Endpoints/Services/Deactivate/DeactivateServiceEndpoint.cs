using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.UseCase.Services.Commands.Deactivate;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.Services.Deactivate;

public sealed class DeactivateServiceEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPatch($"{ServiceApiInfo.Prefix}/{{id:long}}/Deactivate", async (
                    long id,
                    [FromQuery] long salonId,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<DeactivateServiceCommand, bool>(
                        new DeactivateServiceCommand(salonId, id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "خدمت غیرفعال شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
