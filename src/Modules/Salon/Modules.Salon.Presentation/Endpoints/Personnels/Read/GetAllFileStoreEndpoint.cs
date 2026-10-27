using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation.Endpoints.Personnels.Read;



public static class GetAllPersonnelEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{ApiInfo.Prefix}", handler: async (


                    [AsParameters] GetAllPersonnelRequestDto request,
                  [FromServices] IQueryBus _queryBus,
                  HttpContext context,
                  CancellationToken ct
                ) =>
            {
                if (!TryGetSalonId(context, out var salonId))
                    return Unauthorized();

                request.SalonId = salonId;

                var result = await _queryBus.Send<GetAllPersonnelQuery, PagedList<GetAllPersonalResponseDto>>
                                 (new GetAllPersonnelQuery(request), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(result.Value);


            })
                .RequireAuthorization()
                .WithTags(ApiInfo.Tag);
        }
    }
}
