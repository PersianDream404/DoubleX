using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;
using Modules.Banner.Application.Contract.DTOs.Banners.Get;
using Modules.Banner.Application.Contract.UseCase.Banners.Queries;

namespace Modules.Banner.Presentation.Endpoints.Banners.Read;

public static class GetByIdBannerEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{ApiInfo.Prefix}/{{id}}", handler: async (


                    int id,
                  [FromServices] IQueryBus _queryBus, CancellationToken ct
                ) =>
            {

                var result = await _queryBus.Send<GetByIdBannerQuery, GetByIdBannerResponseDto>
                                 (new GetByIdBannerQuery(id),ct);

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