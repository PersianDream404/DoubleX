using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class GetMaterialByIdEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{InventoryApiInfo.Prefix}/Materials/{{id:long}}", async (
                    [FromRoute] long id,
                    [FromServices] IQueryBus queryBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await queryBus.Send<GetMaterialByIdQuery, MaterialDto>(
                        new GetMaterialByIdQuery(id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : NotFound(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
