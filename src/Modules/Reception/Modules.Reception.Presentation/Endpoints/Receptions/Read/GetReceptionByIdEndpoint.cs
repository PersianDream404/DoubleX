using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Reception.Application.Contract.DTOs.Receptions.Get;
using Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetById;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Endpoints.Receptions.Read;

public sealed class GetReceptionByIdEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ReceptionApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    IQueryBus queryBus,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    if (!ReceptionApiInfo.TryGetSalonId(httpContext, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    var result = await queryBus.Send<GetReceptionByIdQuery, ReceptionDetailsDto>(
                        new GetReceptionByIdQuery(id, salonId),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ReceptionApiInfo.Tag);
    }
}
