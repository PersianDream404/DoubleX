using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;
using Modules.Service.Application.Contract.UseCase.ServiceMaterials.Commands.Upsert;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceMaterials.Upsert;

public sealed class UpsertServiceMaterialEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost($"{ServiceApiInfo.Prefix}/Materials", async (
                    [FromBody] ServiceMaterialRequestDto request,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<UpsertServiceMaterialCommand, bool>(
                        new UpsertServiceMaterialCommand(null, request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "ماده استاندارد خدمت ذخیره شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
