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

namespace Modules.Finance.Presentation.Endpoints.Expenses.Write;

public sealed class CancelExpenseEndpoint : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut($"{FinanceApiInfo.Prefix}/expenses/{{id:long}}/cancel",
                        async (
                            long id,
                            [FromBody] CancelExpenseRequestDto request,
                            [FromServices] ICommandBus commandBus,
                            HttpContext context,
                            CancellationToken cancellationToken) =>
                        {
                            if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                                return Unauthorized();
                            var result = await commandBus.Send<CancelExpenseCommand, bool>(
                                new CancelExpenseCommand(salonId, id, request.Reason),
                                cancellationToken);
                            return result.IsSuccess
                                ? Ok(new { id, cancelled = true })
                                : BadRequest(result.GetErrorMessage());
                        })
                    .RequireAuthorization()
                    .WithTags(FinanceApiInfo.Tag);
    }
}
