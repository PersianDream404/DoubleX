using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class CreateUnitEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{InventoryApiInfo.Prefix}/Units", async (
                    [FromBody] CreateUnitRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await commandBus.Send<CreateUnitCommand, bool>(
                        new CreateUnitCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "واحد با موفقیت ثبت شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
