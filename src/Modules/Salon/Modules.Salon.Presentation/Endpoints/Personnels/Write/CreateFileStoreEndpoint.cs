using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using Modules.Personnel.Presentation.Endpoints.Personnels;
using SharedKernel.Constants;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation.Endpoints.Personnels.Write;



public static class CreatePersonnelEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{ApiInfo.Prefix}", handler: async (


                    [FromBody] CreatePersonnelRequestDto request,
                  [FromServices] ICommandBus _commandBus,
                  HttpContext context,
                  CancellationToken ct
                ) =>
            {
                if (!TryGetSalonId(context, out var salonId))
                    return Unauthorized();

                var result = await _commandBus.Send<CreatePersonnelCommand, long>
                                 (new CreatePersonnelCommand(request with { SalonId = salonId }), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(new
                {
                    id = result.Value,
                    message = "پرسنل با موفقیت ثبت شد."
                });


            })
                .RequireAuthorization()
                .WithTags(ApiInfo.Tag);
        }
    }
}