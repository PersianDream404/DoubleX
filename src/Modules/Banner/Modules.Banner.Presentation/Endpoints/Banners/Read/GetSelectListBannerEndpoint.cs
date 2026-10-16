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



public static class GetSelectListBannerEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{ApiInfo.Prefix}/SelectList", handler: async (


                    [AsParameters] GetSelectListBannerRequestDto request,
                  [FromServices] IQueryBus _queryBus, CancellationToken ct
                ) =>
            {

                var result = await _queryBus.Send<GetSelectListBannerQuery, PagedList< GetSelectListBannerResponseDto >>
                                 (new GetSelectListBannerQuery(request),ct);

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
