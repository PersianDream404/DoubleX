using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using SharedKernel.Constants;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation.Endpoints.Personnels.Write;

public static class DeletePersonnelEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete($"{ApiInfo.Prefix}/{{id}}", async (
                long id,
                [FromServices] ICommandBus commandBus,
                [FromServices] IQueryBus queryBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!TryGetSalonId(context, out var salonId))
                    return Unauthorized();

                var personnel = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
                    new GetPersonnelByIdQuery(id), cancellationToken);
                if (!personnel.IsSuccess || personnel.Value!.SalonId != salonId)
                    return NotFound("پرسنل یافت نشد.");

                var result = await commandBus.Send<DeletePersonnelCommand, bool>(
                    new DeletePersonnelCommand(id),
                    cancellationToken);

                if (!result.IsSuccess)
                    return BadRequest(result.GetErrorMessage());

                return Ok(MessageHelper.Format(AppMessages.Delete, AppEntity.Personnel));
            })
            .RequireAuthorization()
            .WithTags(ApiInfo.Tag);
        }
    }
}
