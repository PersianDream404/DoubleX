using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;
using Modules.Service.Application.Contract.UseCase.ServiceMaterials.Commands.Upsert;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceMaterials.Update;

public sealed class UpdateServiceMaterialEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut($"{ServiceApiInfo.Prefix}/Materials/{{id:long}}", async (
                    long id,
                    [FromBody] ServiceMaterialRequestDto request,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<UpsertServiceMaterialCommand, bool>(
                        new UpsertServiceMaterialCommand(id, request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "ماده استاندارد خدمت به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
