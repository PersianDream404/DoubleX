using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Modules.Salon.Application.Contract.DTOs.Salons.Update;
using Modules.Salon.Application.Contract.UseCase.Salons.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Salon.Presentation.Endpoints.Salons.Write;



public static class UpdateSalonEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{ApiInfo.Prefix}/{{id}}", handler: async (

                  long id,
                    [FromBody]UpdateSalonRequestDto request,
                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {

                if (id != request.Id)
                    return BadRequest(AppMessages.BadRequest);

                var result = await _commandBus.Send<UpdateSalonCommand, bool>
                                 (new UpdateSalonCommand(request),ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(MessageHelper.Format(AppMessages.Edit, AppEntity.Salon));


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}