using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class DeactivateMaterialEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch($"{InventoryApiInfo.Prefix}/Materials/{{id:long}}/Deactivate", async (
                    [FromRoute] long id,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await commandBus.Send<DeactivateMaterialCommand, bool>(
                        new DeactivateMaterialCommand(id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "کالا غیرفعال شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
