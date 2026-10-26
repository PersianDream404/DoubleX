using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetByPersonnel;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePersonnels.GetByPersonnel;

public sealed class GetServicePersonnelAssignmentsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet(
                    $"{ServiceApiInfo.Prefix}/Personnels/ByPersonnel",
                    async (
                        [FromQuery] long salonId,
                        [FromQuery] long personnelId,
                        [FromServices] IQueryBus bus,
                        CancellationToken cancellationToken) =>
                    {
                        var result = await bus.Send<
                            GetServicePersonnelAssignmentsQuery,
                            IReadOnlyList<ServicePersonnelDto>>(
                            new GetServicePersonnelAssignmentsQuery(salonId, personnelId),
                            cancellationToken);
                        return result.IsSuccess
                            ? Ok(result.Value!)
                            : BadRequest(result.GetErrorMessage());
                    })
                .WithTags(ServiceApiInfo.Tag);
    }
}
