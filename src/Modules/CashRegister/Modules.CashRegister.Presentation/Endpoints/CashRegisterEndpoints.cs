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

namespace Modules.CashRegister.Presentation.Endpoints;

public sealed class CashRegisterEndpoints : BaseEndpoint, IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(CashRegisterApiInfo.Prefix, async (
                [AsParameters] CashRegisterListRequestDto request,
                [FromServices] IQueryBus queryBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                request.SalonId = salonId;
                var result = await queryBus.Send<GetCashRegistersQuery, Framwork.PagedList.PagedList<CashRegisterDto>>(
                    new GetCashRegistersQuery(request), cancellationToken);
                return result.IsSuccess ? Ok(result.Value) : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

        app.MapPost(CashRegisterApiInfo.Prefix, async (
                [FromBody] CreateCashRegisterRequestDto request,
                [FromServices] ICommandBus commandBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                var result = await commandBus.Send<CreateCashRegisterCommand, long>(
                    new CreateCashRegisterCommand(salonId, request), cancellationToken);
                return result.IsSuccess
                    ? Ok(new { id = result.Value })
                    : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

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

        app.MapPut($"{CashRegisterApiInfo.Prefix}/{{id:long}}", async (
                long id,
                [FromBody] UpdateCashRegisterRequestDto request,
                [FromServices] ICommandBus commandBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                var result = await commandBus.Send<UpdateCashRegisterCommand, bool>(
                    new UpdateCashRegisterCommand(id, salonId, request), cancellationToken);
                return result.IsSuccess ? Ok(new { message = "اطلاعات صندوق به‌روزرسانی شد." }) : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

        app.MapPut($"{CashRegisterApiInfo.Prefix}/{{id:long}}/Toggle", async (
                long id,
                [FromServices] ICommandBus commandBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                var result = await commandBus.Send<ToggleCashRegisterCommand, bool>(
                    new ToggleCashRegisterCommand(id, salonId), cancellationToken);
                return result.IsSuccess ? Ok(new { message = "وضعیت صندوق تغییر کرد." }) : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

        app.MapPost($"{CashRegisterApiInfo.Prefix}/{{id:long}}/sessions", async (
                long id,
                [FromBody] OpenCashRegisterSessionRequestDto request,
                [FromServices] ICommandBus commandBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetIdentity(context, out var salonId, out var userId))
                    return Unauthorized();
                var result = await commandBus.Send<OpenCashRegisterSessionCommand, long>(
                    new OpenCashRegisterSessionCommand(id, salonId, userId, request.OpeningBalance),
                    cancellationToken);
                return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

        app.MapGet($"{CashRegisterApiInfo.Prefix}/{{id:long}}/sessions/active", async (
                long id,
                [FromServices] IQueryBus queryBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                var result = await queryBus.Send<GetCashRegisterSessionQuery, CashRegisterSessionDto>(
                    new GetCashRegisterSessionQuery(id, salonId), cancellationToken);
                return result.IsSuccess ? Ok(result.Value) : NotFound(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

        app.MapGet($"{CashRegisterApiInfo.Prefix}/{{id:long}}/sessions", async (
                long id,
                int? pageNumber,
                int? pageSize,
                [FromServices] IQueryBus queryBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                var result = await queryBus.Send<
                    GetCashRegisterSessionsQuery,
                    Framwork.PagedList.PagedList<CashRegisterSessionDto>>(
                    new GetCashRegisterSessionsQuery(id, salonId, pageNumber ?? 1, pageSize ?? 50),
                    cancellationToken);
                return result.IsSuccess ? Ok(result.Value) : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

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

        app.MapGet($"{CashRegisterApiInfo.Prefix}/{{id:long}}/balance", async (
                long id,
                [FromServices] IQueryBus queryBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                var result = await queryBus.Send<GetCashRegisterBalanceQuery, CashRegisterBalanceDto>(
                    new GetCashRegisterBalanceQuery(id, salonId), cancellationToken);
                return result.IsSuccess ? Ok(result.Value) : NotFound(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

        app.MapGet($"{CashRegisterApiInfo.Prefix}/{{id:long}}/transactions", async (
                long id,
                [AsParameters] CashRegisterTransactionListRequestDto request,
                [FromServices] IQueryBus queryBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetSalonId(context, out var salonId))
                    return Unauthorized();
                request.SalonId = salonId;
                var result = await queryBus.Send<
                    GetCashRegisterTransactionsQuery,
                    Framwork.PagedList.PagedList<CashRegisterTransactionDto>>(
                    new GetCashRegisterTransactionsQuery(id, request), cancellationToken);
                return result.IsSuccess ? Ok(result.Value) : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);

        MapMovement(app, "cash-in", CashRegisterTransactionType.CashIn);
        MapMovement(app, "cash-out", CashRegisterTransactionType.CashOut);
    }

    private void MapMovement(
        IEndpointRouteBuilder app,
        string route,
        CashRegisterTransactionType type) =>
        app.MapPost($"{CashRegisterApiInfo.Prefix}/{{id:long}}/{route}", async (
                long id,
                [FromBody] CashMovementRequestDto request,
                [FromServices] ICommandBus commandBus,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                if (!CashRegisterApiInfo.TryGetIdentity(context, out var salonId, out var userId))
                    return Unauthorized();
                var result = await commandBus.Send<RecordCashMovementCommand, bool>(
                    new RecordCashMovementCommand(
                        id, salonId, userId, type, request.Amount, request.IdempotencyKey, request.Description),
                    cancellationToken);
                return result.IsSuccess ? Ok(new { message = "گردش صندوق ثبت شد." }) : BadRequest(result.GetErrorMessage());
            })
            .RequireAuthorization()
            .WithTags(CashRegisterApiInfo.Tag);
}
