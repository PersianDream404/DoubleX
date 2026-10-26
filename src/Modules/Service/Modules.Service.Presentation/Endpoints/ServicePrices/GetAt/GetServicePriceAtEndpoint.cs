using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Application.Contract.UseCase.ServicePrices.Queries.GetAt;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePrices.GetAt;

public sealed class GetServicePriceAtEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Prices/Current", async (
                    [FromQuery] long salonId,
                    [FromQuery] long serviceId,
                    [FromQuery] DateTimeOffset at,
                    [FromServices] IQueryBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<GetServicePriceAtQuery, ServicePriceDto>(
                        new GetServicePriceAtQuery(salonId, serviceId, at), cancellationToken);
                    return result.IsSuccess ? Ok(result.Value!) : NotFound(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
