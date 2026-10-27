using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Domain.Entities;
using Modules.CashRegister.Persistence.Context;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Persistence.Repositories;

public sealed class CashRegisterCommandRepository(CashRegisterWriteDbContext context)
    : ICashRegisterCommandRepository
{
    public async Task<bool> AddRegisterAsync(CashRegisterEntity register, CancellationToken cancellationToken)
    {
        await context.CashRegisters.AddAsync(register, cancellationToken);
        return await SaveChangesAsync(cancellationToken);
    }

    public Task<bool> IsCodeUsedAsync(
        long salonId,
        string code,
        long? exceptId,
        CancellationToken cancellationToken) =>
        context.CashRegisters.AnyAsync(
            register => register.SalonId == salonId &&
                        register.Code == code.Trim().ToUpper() &&
                        (!exceptId.HasValue || register.Id != exceptId.Value),
            cancellationToken);

    public Task<CashRegisterEntity?> GetRegisterAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken) =>
        context.CashRegisters.SingleOrDefaultAsync(
            register => register.Id == id && register.SalonId == salonId,
            cancellationToken);

    public Task<CashRegisterSession?> GetOpenSessionAsync(
        long cashRegisterId,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Sessions.SingleOrDefaultAsync(
            session => session.CashRegisterId == cashRegisterId &&
                       session.SalonId == salonId &&
                       session.SessionStatus == CashRegisterSessionStatus.Open,
            cancellationToken);

    public Task<CashRegisterSession?> GetSessionAsync(
        long sessionId,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Sessions.SingleOrDefaultAsync(
            session => session.Id == sessionId && session.SalonId == salonId,
            cancellationToken);

    public Task<bool> IsTransactionIdempotencyKeyUsedAsync(
        long salonId,
        string key,
        CancellationToken cancellationToken) =>
        context.Transactions.AnyAsync(
            transaction => transaction.SalonId == salonId && transaction.IdempotencyKey == key,
            cancellationToken);

    public async Task<bool> AddSessionWithOpeningTransactionAsync(
        CashRegisterSession session,
        CancellationToken cancellationToken)
    {
        await context.Sessions.AddAsync(session, cancellationToken);
        return await SaveChangesAsync(cancellationToken);
    }

    public async Task AddTransactionAsync(
        CashRegisterTransaction transaction,
        CancellationToken cancellationToken) =>
        await context.Transactions.AddAsync(transaction, cancellationToken);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            context.ChangeTracker.Clear();
            return false;
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            context.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task<decimal> GetExpectedBalanceAsync(long sessionId, CancellationToken cancellationToken)
    {
        var openingBalance = await context.Sessions
            .Where(session => session.Id == sessionId)
            .Select(session => session.OpeningBalance)
            .SingleOrDefaultAsync(cancellationToken);
        var inflows = await context.Transactions
            .Where(transaction => transaction.SessionId == sessionId &&
                                 (transaction.Type == CashRegisterTransactionType.CashIn ||
                                  transaction.Type == CashRegisterTransactionType.ReceptionPayment))
            .SumAsync(transaction => (decimal?)transaction.Amount, cancellationToken) ?? 0m;
        var outflows = await context.Transactions
            .Where(transaction => transaction.SessionId == sessionId &&
                                 transaction.Type == CashRegisterTransactionType.CashOut)
            .SumAsync(transaction => (decimal?)transaction.Amount, cancellationToken) ?? 0m;
        return openingBalance + inflows - outflows;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}
