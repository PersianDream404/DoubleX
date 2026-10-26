using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetById;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePersonnels.GetById;

public sealed class GetServicePersonnelByIdEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Personnels/{{id:long}}", async (
                    long id,
                    [FromQuery] long salonId,
                    [FromQuery] long serviceId,
                    [FromServices] IQueryBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<GetServicePersonnelByIdQuery, ServicePersonnelDto>(
                        new GetServicePersonnelByIdQuery(salonId, serviceId, id),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value!)
                        : NotFound(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
