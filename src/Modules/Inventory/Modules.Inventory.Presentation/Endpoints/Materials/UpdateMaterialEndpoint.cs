using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class UpdateMaterialEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{InventoryApiInfo.Prefix}/Materials/{{id:long}}", async (
                    [FromRoute] long id,
                    [FromBody] UpdateMaterialRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    if (id != request.Id)
                    {
                        return BadRequest("شناسهٔ مسیر و درخواست یکسان نیستند.");
                    }

                    var result = await commandBus.Send<UpdateMaterialCommand, bool>(
                        new UpdateMaterialCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "کالا به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
