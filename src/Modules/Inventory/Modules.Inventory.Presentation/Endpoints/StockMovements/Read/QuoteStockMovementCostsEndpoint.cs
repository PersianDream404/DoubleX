using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.UseCase;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Endpoints;

public sealed class QuoteStockMovementCostsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{InventoryApiInfo.Prefix}/Movements/CostQuote", async (
                    [FromBody] QuoteStockMovementCostsRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await queryBus.Send<
                        QuoteStockMovementCostsQuery,
                        QuoteStockMovementCostsResponseDto>(
                        new QuoteStockMovementCostsQuery(request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(InventoryApiInfo.Tag);
        }
    }
}
