using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Application.Contract.UseCase.ServicePrices.Queries.GetHistory;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePrices.GetHistory;

public sealed class GetServicePriceHistoryEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Prices/History", async (
                    [FromQuery] long salonId,
                    [FromQuery] long serviceId,
                    [FromServices] IQueryBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<GetServicePriceHistoryQuery, IReadOnlyList<ServicePriceDto>>(
                        new GetServicePriceHistoryQuery(salonId, serviceId), cancellationToken);
                    return result.IsSuccess ? Ok(result.Value!) : NotFound(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
