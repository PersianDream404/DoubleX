using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class DeleteMaterialEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete($"{InventoryApiInfo.Prefix}/Materials/{{id:long}}", async (
                    [FromRoute] long id,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await commandBus.Send<DeleteMaterialCommand, bool>(
                        new DeleteMaterialCommand(id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "کالا حذف نرم شد." })
                        : NotFound(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
