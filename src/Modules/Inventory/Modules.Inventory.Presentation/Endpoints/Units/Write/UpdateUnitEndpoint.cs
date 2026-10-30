using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.DTOs.Units;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class UpdateUnitEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{InventoryApiInfo.Prefix}/Units/{{id:long}}", async (
                    [FromRoute] long id,
                    [FromBody] UpdateUnitRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    if (id != request.Id)
                    {
                        return BadRequest("شناسهٔ مسیر و درخواست یکسان نیستند.");
                    }

                    var result = await commandBus.Send<UpdateUnitCommand, bool>(
                        new UpdateUnitCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "واحد به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
