using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Modules.Salon.Application.Contract.DTOs.Salons.Create;
using Modules.Salon.Application.Contract.UseCase.Salons.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Salon.Presentation.Endpoints.Salons.Write;



public static class CreateSalonEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{ApiInfo.Prefix}", handler: async (


                    [FromBody]CreateSalonRequestDto request,
                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {

                var result = await _commandBus.Send<CreateSalonCommand, bool>
                                 (new CreateSalonCommand(request), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(MessageHelper.Format(AppMessages.Create, AppEntity.Salon));


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}