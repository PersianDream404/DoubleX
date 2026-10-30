using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetAll;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.Services.GetAll;

public sealed class GetServicesEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet(ServiceApiInfo.Prefix, async (
                    [FromServices] IQueryBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<GetServicesQuery, IReadOnlyList<ServiceDto>>(
                        new GetServicesQuery(salonId), cancellationToken);
                    return result.IsSuccess ? Ok(result.Value!) : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}
