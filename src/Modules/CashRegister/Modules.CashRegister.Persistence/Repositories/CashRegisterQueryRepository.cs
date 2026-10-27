using Framwork.PagedList;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Domain.Entities;
using Modules.CashRegister.Persistence.Context;
using Modules.CashRegister.Persistence.Mapper;

namespace Modules.CashRegister.Persistence.Repositories;

public sealed class CashRegisterQueryRepository(CashRegisterReadDbContext context)
    : ICashRegisterQueryRepository
{
    public async Task<PagedList<CashRegisterDto>> GetRegistersAsync(
        CashRegisterListRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.CashRegisters
            .AsNoTracking()
            .Where(register => register.SalonId == request.SalonId);
        if (request.IsActive.HasValue)
            query = query.Where(register => register.Status == request.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var term = request.Q.Trim();
            query = query.Where(register => register.Name.Contains(term) || register.Code.Contains(term));
        }

        return await query
            .OrderBy(register => register.Name)
            .Select(CashRegisterMapper.ToCashRegisterDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public Task<CashRegisterDto?> GetRegisterAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken) =>
        context.CashRegisters
            .AsNoTracking()
            .Where(register => register.Id == id && register.SalonId == salonId)
            .Select(CashRegisterMapper.ToCashRegisterDto())
            .SingleOrDefaultAsync(cancellationToken);

    public Task<CashRegisterSessionDto?> GetOpenSessionAsync(
        long cashRegisterId,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Sessions
            .AsNoTracking()
            .Where(session => session.CashRegisterId == cashRegisterId &&
                              session.SalonId == salonId &&
                              session.SessionStatus == CashRegisterSessionStatus.Open)
            .Select(CashRegisterMapper.ToSessionDto())
            .SingleOrDefaultAsync(cancellationToken);

    public Task<CashRegisterSessionDto?> GetSessionAsync(
        long sessionId,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Sessions
            .AsNoTracking()
            .Where(session => session.Id == sessionId && session.SalonId == salonId)
            .Select(CashRegisterMapper.ToSessionDto())
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<PagedList<CashRegisterSessionDto>> GetSessionsAsync(
        long cashRegisterId,
        long salonId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken) =>
        await context.Sessions
            .AsNoTracking()
            .Where(session => session.CashRegisterId == cashRegisterId && session.SalonId == salonId)
            .OrderByDescending(session => session.OpenedAt)
            .Select(CashRegisterMapper.ToSessionDto())
            .ToPagedList(pageNumber, pageSize, cancellationToken);

    public async Task<PagedList<CashRegisterTransactionDto>> GetTransactionsAsync(
        long cashRegisterId,
        CashRegisterTransactionListRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.CashRegisterId == cashRegisterId &&
                                  transaction.SalonId == request.SalonId);
        if (request.SessionId.HasValue)
            query = query.Where(transaction => transaction.SessionId == request.SessionId.Value);
        if (request.Type.HasValue)
            query = query.Where(transaction => transaction.Type == request.Type.Value);
        if (request.From.HasValue)
            query = query.Where(transaction => transaction.OccurredAt >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(transaction => transaction.OccurredAt <= request.To.Value);

        return await query
            .OrderByDescending(transaction => transaction.OccurredAt)
            .Select(CashRegisterMapper.ToTransactionDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public async Task<CashRegisterBalanceDto?> GetBalanceAsync(
        long cashRegisterId,
        long salonId,
        CancellationToken cancellationToken)
    {
        var session = await context.Sessions
            .AsNoTracking()
            .Where(item => item.CashRegisterId == cashRegisterId &&
                           item.SalonId == salonId &&
                           item.SessionStatus == CashRegisterSessionStatus.Open)
            .Select(item => new { item.Id, item.OpeningBalance })
            .SingleOrDefaultAsync(cancellationToken);
        if (session is null)
            return null;

        var inflows = await context.Transactions
            .Where(transaction => transaction.SessionId == session.Id &&
                                 (transaction.Type == CashRegisterTransactionType.CashIn ||
                                  transaction.Type == CashRegisterTransactionType.ReceptionPayment))
            .SumAsync(transaction => (decimal?)transaction.Amount, cancellationToken) ?? 0m;
        var outflows = await context.Transactions
            .Where(transaction => transaction.SessionId == session.Id &&
                                 transaction.Type == CashRegisterTransactionType.CashOut)
            .SumAsync(transaction => (decimal?)transaction.Amount, cancellationToken) ?? 0m;
        return new CashRegisterBalanceDto(cashRegisterId, session.Id, session.OpeningBalance + inflows - outflows);
    }

    public Task<CashRegisterTransaction?> GetTransactionByIdempotencyKeyAsync(
        long salonId,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        context.Transactions
            .AsNoTracking()
            .SingleOrDefaultAsync(
                transaction => transaction.SalonId == salonId &&
                               transaction.IdempotencyKey == idempotencyKey,
                cancellationToken);
}
