using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Commands.Upsert;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePersonnels.Update;

public sealed class UpdateServicePersonnelEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut($"{ServiceApiInfo.Prefix}/Personnels/{{id:long}}", async (
                    long id,
                    [FromBody] ServicePersonnelRequestDto request,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<UpsertServicePersonnelCommand, bool>(
                        new UpsertServicePersonnelCommand(id, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "پرسنل اختصاص‌یافته به خدمت به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
