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

namespace Modules.CashRegister.Presentation.Endpoints.CashRegisters.Read;

public sealed class GetCashRegisterByIdEndpoint : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{CashRegisterApiInfo.Prefix}/{{id:long}}", async (
                        long id,
                        [FromServices] IQueryBus queryBus,
                        HttpContext context,
                        CancellationToken cancellationToken) =>
                    {
                        if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                            return Unauthorized();
                        var result = await queryBus.Send<GetCashRegisterByIdQuery, CashRegisterDto>(
                            new GetCashRegisterByIdQuery(id, salonId), cancellationToken);
                        return result.IsSuccess ? Ok(result.Value) : NotFound(result.GetErrorMessage());
                    })
                    .RequireAuthorization()
                    .WithTags(CashRegisterApiInfo.Tag);
    }
}
