using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Commands.Upsert;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePersonnels.Create;

public sealed class CreateServicePersonnelEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost($"{ServiceApiInfo.Prefix}/Personnels", async (
                    [FromBody] ServicePersonnelRequestDto request,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<UpsertServicePersonnelCommand, bool>(
                        new UpsertServicePersonnelCommand(null, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "پرسنل به خدمت اختصاص داده شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
