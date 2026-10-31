using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Dashboard.Application.Contract.DTOs;
using Modules.Dashboard.Application.Contract.UseCase.Queries.GetAlerts;
using SharedKernel.Interface;

namespace Modules.Dashboard.Presentation.Endpoints.Dashboard.Read;

public sealed class GetDashboardAlertsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{DashboardApiInfo.Prefix}/alerts", async (
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!DashboardApiInfo.TryGetSalonId(context, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    var result = await queryBus.Send<
                        GetDashboardAlertsQuery,
                        IReadOnlyList<DashboardAlertDto>>(
                        new GetDashboardAlertsQuery(salonId),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(DashboardApiInfo.Tag);
    }
}
