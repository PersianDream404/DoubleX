using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;
using Modules.FileStore.Application.Contract.DTOs.FileStores.GetAll;
using Modules.FileStore.Application.Contract.UseCase.FileStores.Queries;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Get;


namespace Modules.FileStore.Presentation.Endpoints.FileStores.Read;

public static class GetByIdFileStoreEndpoint
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

                var result = await _queryBus.Send<GetByIdFileStoreQuery, GetByIdFileStoreResponseDto>
                                 (new GetByIdFileStoreQuery(id), ct);

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