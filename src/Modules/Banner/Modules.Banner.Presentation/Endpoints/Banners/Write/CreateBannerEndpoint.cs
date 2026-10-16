using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Banner.Application.Contract.DTOs.Banners.Create;
using Modules.Banner.Application.Contract.UseCase.Banners.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Banner.Presentation.Endpoints.Banners.Write;



public static class CreateBannerEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{ApiInfo.Prefix}", handler: async (


                    [FromBody]CreateBannerRequestDto request,
                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {

                var result = await _commandBus.Send<CreateBannerCommand, bool>
                                 (new CreateBannerCommand(request), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(MessageHelper.Format(AppMessages.Create, AppEntity.Banner));


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}