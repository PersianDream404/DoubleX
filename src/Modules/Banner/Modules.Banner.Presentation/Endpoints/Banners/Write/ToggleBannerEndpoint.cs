using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Banner.Application.Contract.UseCase.Banners.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Banner.Presentation.Endpoints.Banners.Write;



public static class ToggleBannerEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{ApiInfo.Prefix}/{{id}}/Toggle", handler: async (

                  long id,
                 
                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {


                var result = await _commandBus.Send<ToggleBannerCommand, bool>
                                 (new ToggleBannerCommand(id), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(MessageHelper.Format(AppMessages.Toggle, AppEntity.Banner));


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}