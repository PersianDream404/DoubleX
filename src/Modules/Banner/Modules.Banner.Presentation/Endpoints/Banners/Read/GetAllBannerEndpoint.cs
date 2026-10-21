using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;
using Modules.Banner.Application.Contract.DTOs.Banners.GetAll;
using Modules.Banner.Application.Contract.UseCase.Banners.Queries;

namespace Modules.Banner.Presentation.Endpoints.Banners.Read;



public static class GetAllBannerEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{ApiInfo.Prefix}", handler: async (


                    [AsParameters] GetAllBannerRequestDto request,
                  [FromServices] IQueryBus _queryBus,CancellationToken ct
                ) =>
            {

                var result = await _queryBus.Send<GetAllBannerQuery, PagedList< GetAllBannerResponseDto >>
                                 (new GetAllBannerQuery(request), ct);

                if (!result.IsSuccess)
                {
                    var message = result.GetErrorMessage();
                    return BadRequest(message);
                }

                return Ok(result.Value);


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}
