using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetAll;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePersonnels.GetAll;

public sealed class GetServicePersonnelsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Personnels", async (
                    [FromQuery] long salonId,
                    [FromQuery] long serviceId,
                    [FromServices] IQueryBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<
                        GetServicePersonnelsQuery,
                        IReadOnlyList<ServicePersonnelDto>>(
                        new GetServicePersonnelsQuery(salonId, serviceId),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value!)
                        : NotFound(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
