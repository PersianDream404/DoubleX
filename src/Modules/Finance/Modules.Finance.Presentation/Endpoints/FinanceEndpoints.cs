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

namespace Modules.Finance.Presentation;

public sealed class FinanceEndpoints : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"{FinanceApiInfo.Prefix}/earnings/receptions/{{receptionId:long}}/calculate",
                async (
                    long receptionId,
                    [FromServices] ICommandBus commandBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await commandBus.Send<CalculateReceptionEarningsCommand, int>(
                        new CalculateReceptionEarningsCommand(salonId, receptionId),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { createdCount = result.Value })
                        : BadRequest(result.GetErrorMessage());
                })
            .RequireAuthorization()
            .WithTags(FinanceApiInfo.Tag);

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

        app.MapPost($"{FinanceApiInfo.Prefix}/expenses",
                async (
                    [FromBody] CreateExpenseRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!FinanceApiInfo.TryGetIdentity(context, out var salonId, out var userId))
                        return Unauthorized();
                    var result = await commandBus.Send<CreateExpenseCommand, long>(
                        new CreateExpenseCommand(salonId, userId, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { id = result.Value })
                        : BadRequest(result.GetErrorMessage());
                })
            .RequireAuthorization()
            .WithTags(FinanceApiInfo.Tag);

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

        app.MapGet($"{FinanceApiInfo.Prefix}/expenses",
                async (
                    [AsParameters] GetExpensesRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await queryBus.Send<GetExpensesQuery,
                        Framwork.PagedList.PagedList<ExpenseListItemDto>>(
                        new GetExpensesQuery(salonId, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
            .RequireAuthorization()
            .WithTags(FinanceApiInfo.Tag);

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

        app.MapGet($"{FinanceApiInfo.Prefix}/reports/expenses",
                async (
                    [AsParameters] GetExpensesRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await queryBus.Send<GetExpenseReportQuery, ExpenseReportDto>(
                        new GetExpenseReportQuery(salonId, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
            .RequireAuthorization()
            .WithTags(FinanceApiInfo.Tag);

        app.MapGet($"{FinanceApiInfo.Prefix}/settlements",
                async (
                    [AsParameters] GetPersonnelSettlementsRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await queryBus.Send<GetPersonnelSettlementsQuery,
                        Framwork.PagedList.PagedList<PersonnelSettlementListItemDto>>(
                        new GetPersonnelSettlementsQuery(salonId, request),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
            .RequireAuthorization()
            .WithTags(FinanceApiInfo.Tag);

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

        app.MapGet($"{FinanceApiInfo.Prefix}/reports/summary",
                async (
                    DateTimeOffset? from,
                    DateTimeOffset? to,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!FinanceApiInfo.TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    var result = await queryBus.Send<GetFinanceSummaryQuery, FinanceSummaryDto>(
                        new GetFinanceSummaryQuery(salonId, from, to),
                        cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
            .RequireAuthorization()
            .WithTags(FinanceApiInfo.Tag);
    }
}
