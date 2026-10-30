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

namespace Modules.Finance.Presentation.Endpoints.Earnings.Write;

public sealed class CalculateFixedSalaryEndpoint : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"{FinanceApiInfo.Prefix}/earnings/fixed-salary",
                        async (
                            [FromBody] CalculateFixedSalaryRequestDto request,
                            [FromServices] ICommandBus commandBus,
                            HttpContext context,
                            CancellationToken cancellationToken) =>
                        {
                            if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                                return Unauthorized();
                            var result = await commandBus.Send<CalculateFixedSalaryCommand, long>(
                                new CalculateFixedSalaryCommand(
                                    salonId,
                                    request.PersonnelId,
                                    request.PeriodStart,
                                    request.PeriodEnd),
                                cancellationToken);
                            return result.IsSuccess
                                ? Ok(new { id = result.Value })
                                : BadRequest(result.GetErrorMessage());
                        })
                    .RequireAuthorization()
                    .WithTags(FinanceApiInfo.Tag);
    }
}
