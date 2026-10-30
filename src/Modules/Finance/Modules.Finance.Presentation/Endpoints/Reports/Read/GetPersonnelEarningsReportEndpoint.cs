using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Finance.Application.Contract.DTOs;
using Modules.Finance.Application.Contract.UseCase.Commands;
using Modules.Finance.Application.Contract.UseCase.Queries;
using Modules.Finance.Presentation.Endpoints;
using SharedKernel.Interface;

namespace Modules.Finance.Presentation.Endpoints.Reports.Read;

public sealed class GetPersonnelEarningsReportEndpoint : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{FinanceApiInfo.Prefix}/reports/personnel/{{personnelId:long}}/earnings",
                        async (
                            long personnelId,
                            DateTimeOffset? from,
                            DateTimeOffset? to,
                            [FromServices] IQueryBus queryBus,
                            HttpContext context,
                            CancellationToken cancellationToken) =>
                        {
                            if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                                return Unauthorized();
                            var result = await queryBus.Send<GetPersonnelEarningsReportQuery,
                                PersonnelEarningsReportDto>(
                                new GetPersonnelEarningsReportQuery(salonId, personnelId, from, to),
                                cancellationToken);
                            return result.IsSuccess
                                ? Ok(result.Value)
                                : BadRequest(result.GetErrorMessage());
                        })
                    .RequireAuthorization()
                    .WithTags(FinanceApiInfo.Tag);
    }
}
