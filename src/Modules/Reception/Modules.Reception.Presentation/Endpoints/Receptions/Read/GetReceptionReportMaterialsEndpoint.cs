using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Reception.Application.Contract.DTOs.Reports;
using Modules.Reception.Application.Contract.UseCase.Reports.Queries;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Endpoints.Receptions.Read;

public sealed class GetReceptionReportMaterialsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ReceptionApiInfo.Prefix}/reports/materials", async (
                    [AsParameters] GetReceptionReportMaterialsRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    if (!ReceptionApiInfo.TryGetSalonId(httpContext, out var salonId))
                    {
                        return BadRequest("شناسهٔ سالن از هویت کاربر دریافت نشد.");
                    }

                    request.SalonId = salonId;
                    var result = await queryBus.Send<
                        GetReceptionReportMaterialsQuery,
                        ReceptionReportPagedDto<ReceptionReportMaterialDto, ReceptionReportMaterialTotalsDto>>(
                        new GetReceptionReportMaterialsQuery(request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ReceptionApiInfo.Tag);
    }
}
