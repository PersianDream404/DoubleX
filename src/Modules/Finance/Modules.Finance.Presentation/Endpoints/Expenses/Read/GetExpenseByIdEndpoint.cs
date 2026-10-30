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

namespace Modules.Finance.Presentation.Endpoints.Expenses.Read;

public sealed class GetExpenseByIdEndpoint : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{FinanceApiInfo.Prefix}/expenses/{{id:long}}",
                        async (
                            long id,
                            [FromServices] IQueryBus queryBus,
                            HttpContext context,
                            CancellationToken cancellationToken) =>
                        {
                            if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                                return Unauthorized();
                            var result = await queryBus.Send<GetExpenseByIdQuery, ExpenseListItemDto>(
                                new GetExpenseByIdQuery(salonId, id),
                                cancellationToken);
                            return result.IsSuccess
                                ? Ok(result.Value)
                                : NotFound(result.GetErrorMessage());
                        })
                    .RequireAuthorization()
                    .WithTags(FinanceApiInfo.Tag);
    }
}
