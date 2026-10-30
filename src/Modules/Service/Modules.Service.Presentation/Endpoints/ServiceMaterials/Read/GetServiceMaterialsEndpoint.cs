using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;
using Modules.Service.Application.Contract.UseCase.ServiceMaterials.Queries.GetAll;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceMaterials.GetAll;

public sealed class GetServiceMaterialsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Materials", async (
                    [FromQuery] long serviceId,
                    [FromServices] IQueryBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<GetServiceMaterialsQuery, IReadOnlyList<ServiceMaterialDto>>(
                        new GetServiceMaterialsQuery(salonId, serviceId), cancellationToken);
                    return result.IsSuccess ? Ok(result.Value!) : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}
