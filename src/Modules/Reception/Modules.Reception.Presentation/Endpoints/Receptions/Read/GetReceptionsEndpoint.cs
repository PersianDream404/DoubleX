using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Reception.Application.Contract.DTOs.Receptions.GetAll;
using Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetAll;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Endpoints.Receptions.Read;

public sealed class GetReceptionsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet(ReceptionApiInfo.Prefix, async (
                    [AsParameters] GetReceptionsRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    if (!ReceptionApiInfo.TryGetSalonId(httpContext, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    request.SalonId = salonId;
                    var result = await queryBus.Send<GetReceptionsQuery, PagedList<ReceptionListItemDto>>(
                        new GetReceptionsQuery(request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ReceptionApiInfo.Tag);
    }
}
