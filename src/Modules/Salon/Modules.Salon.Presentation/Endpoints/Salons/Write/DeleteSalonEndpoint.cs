using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Modules.Salon.Application.Contract.UseCase.Salons.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Salon.Presentation.Endpoints.Salons.Write;



public static class DeleteSalonEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete($"{ApiInfo.Prefix}/{{id}}", handler: async (

                  long id,
                   
                  [FromServices] ICommandBus _commandBus, CancellationToken ct
                ) =>
            {


                var result = await _commandBus.Send<DeleteSalonCommand, bool>
                                 (new DeleteSalonCommand(id), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(MessageHelper.Format(AppMessages.Delete, AppEntity.Salon));


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}