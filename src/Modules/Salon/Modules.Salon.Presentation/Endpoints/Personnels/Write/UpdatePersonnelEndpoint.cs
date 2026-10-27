using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation.Endpoints.Personnels.Write;

public static class UpdatePersonnelEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{ApiInfo.Prefix}/{{id}}", async (
                long id,
                [FromBody] UpdatePersonnelRequestDto request,
                [FromServices] ICommandBus commandBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!TryGetSalonId(context, out var salonId))
                    return Unauthorized();

                var result = await commandBus.Send<UpdatePersonnelCommand, bool>(
                    new UpdatePersonnelCommand(id, request with { SalonId = salonId }),
                    cancellationToken);

                if (!result.IsSuccess)
                    return BadRequest(result.GetErrorMessage());

                return Ok(MessageHelper.Format(AppMessages.Edit, AppEntity.Personnel));
            })
            .RequireAuthorization()
            .WithTags(ApiInfo.Tag);
        }
    }
}
