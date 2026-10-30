using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class GetMaterialsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{InventoryApiInfo.Prefix}/Materials", async (
                    [AsParameters] GetMaterialsRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await queryBus.Send<GetMaterialsQuery, PagedList<MaterialDto>>(
                        new GetMaterialsQuery(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
