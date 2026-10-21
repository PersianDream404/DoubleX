using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;
using Modules.Salon.Application.Contract.DTOs.Salons.GetAll;
using Modules.Salon.Application.Contract.UseCase.Salons.Queries;

namespace Modules.Salon.Presentation.Endpoints.Salons.Read;



public static class GetAllSalonEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{ApiInfo.Prefix}", handler: async (


                    [AsParameters] GetAllSalonRequestDto request,
                  [FromServices] IQueryBus _queryBus,CancellationToken ct
                ) =>
            {

                var result = await _queryBus.Send<GetAllSalonQuery, PagedList< GetAllSalonResponseDto >>
                                 (new GetAllSalonQuery(request), ct);

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
