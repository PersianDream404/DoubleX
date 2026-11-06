using Modules.Service.Application.Contract.Security;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetAll;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceTariffs.GetAll;

public sealed class GetServiceTariffsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Tariffs", async (
                    [FromServices] IQueryBus bus,
                    HttpContext context,
                    [FromQuery] string? q,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<GetServiceTariffsQuery, IReadOnlyList<ServiceTariffDto>>(
                        new GetServiceTariffsQuery(salonId, q), cancellationToken);
                    return result.IsSuccess ? Ok(result.Value!) : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization(ServicePermissions.Tariffs)
                .WithTags(ServiceApiInfo.Tag);
    }
}
