using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;
using Modules.CashRegister.Domain.Entities;
using SharedKernel.Interface;
using Modules.CashRegister.Presentation.Endpoints;

namespace Modules.CashRegister.Presentation.Endpoints.CashRegisters.Write;

public sealed class CloseCashRegisterSessionEndpoint : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"{CashRegisterApiInfo.Prefix}/{{id:long}}/sessions/{{sessionId:long}}/close", async (
                        long id,
                        long sessionId,
                        [FromBody] CloseCashRegisterSessionRequestDto request,
                        [FromServices] ICommandBus commandBus,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        if (!CashRegisterApiInfo.TryGetIdentity(context, out var salonId, out var userId))
                            return Unauthorized();
                        var result = await commandBus.Send<CloseCashRegisterSessionCommand, bool>(
                            new CloseCashRegisterSessionCommand(
                                id, sessionId, salonId, userId, request.ActualClosingBalance),
                            cancellationToken);
                        return result.IsSuccess ? Ok(new { message = "صندوق بسته شد." }) : BadRequest(result.GetErrorMessage());
                    })
                    .RequireAuthorization()
                    .WithTags(CashRegisterApiInfo.Tag);
    }
}
