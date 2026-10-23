using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Personnel.Presentation.Endpoints.Personnels;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation.Endpoints.Personnels.Read;

public static class GetByIdPersonnelEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{ApiInfo.Prefix}/{{id}}", handler: async (


                    long id,
                  [FromServices] IQueryBus _queryBus, CancellationToken ct
                ) =>
            {

                var result = await _queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>
                                 (new GetPersonnelByIdQuery(id), ct);

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