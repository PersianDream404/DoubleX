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

namespace Modules.Finance.Presentation.Endpoints.Settlements.Write;

public sealed class RegisterPersonnelSettlementEndpoint : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"{FinanceApiInfo.Prefix}/settlements",
                        async (
                            [FromBody] RegisterPersonnelSettlementRequestDto request,
                            [FromServices] ICommandBus commandBus,
                            HttpContext context,
                            CancellationToken cancellationToken) =>
                        {
                            if (!FinanceApiInfo.TryGetIdentity(context, out var salonId, out var userId))
                                return Unauthorized();
                            var result = await commandBus.Send<RegisterPersonnelSettlementCommand, long>(
                                new RegisterPersonnelSettlementCommand(salonId, userId, request),
                                cancellationToken);
                            return result.IsSuccess
                                ? Ok(new { id = result.Value })
                                : BadRequest(result.GetErrorMessage());
                        })
                    .RequireAuthorization()
                    .WithTags(FinanceApiInfo.Tag);
    }
}
