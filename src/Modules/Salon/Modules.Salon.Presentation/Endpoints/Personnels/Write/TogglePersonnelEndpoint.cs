using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation.Endpoints.Personnels.Write;

public static class TogglePersonnelEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{ApiInfo.Prefix}/{{id}}/Toggle", async (
                long id,
                [FromServices] ICommandBus commandBus,
                CancellationToken cancellationToken) =>
            {
                var result = await commandBus.Send<TogglePersonnelCommand, bool>(
                    new TogglePersonnelCommand(id),
                    cancellationToken);

                if (!result.IsSuccess)
                    return BadRequest(result.GetErrorMessage());

                return Ok(MessageHelper.Format(AppMessages.Toggle, AppEntity.Personnel));
            })
            .WithTags(ApiInfo.Tag);
        }
    }
}
