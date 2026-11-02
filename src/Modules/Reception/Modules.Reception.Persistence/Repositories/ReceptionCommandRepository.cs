using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Storage;
using Modules.Reception.Application.Contract.DTOs.Payments.Register;
using Modules.Reception.Application.Contract.DTOs;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Domain.Entities;
using Modules.Reception.Persistence.Context;
using Modules.Inventory.Persistence.Context;
using Ardalis.Result;
using System.Data;
using System.Globalization;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Persistence.Repositories;

public sealed class ReceptionCommandRepository(
    ReceptionWriteDbContext context,
    InventoryWriteDbContext inventoryContext)
    : IReceptionCommandRepository, ICashRegisterPaymentOutboxRepository,
        IPersonnelCommissionOutboxRepository
{
    public void AddCashRegisterPaymentOutbox(CashRegisterPaymentOutbox outbox) =>
        context.CashRegisterPaymentOutbox.Add(outbox);

    public void AddPersonnelCommissionOutbox(PersonnelCommissionOutbox outbox) =>
        context.PersonnelCommissionOutbox.Add(outbox);

    public Task<ReceptionEntity?> GetTrackedAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Receptions
            .Include(reception => reception.Services)
                .ThenInclude(service => service.Materials)
            .Include(reception => reception.Payments)
            .SingleOrDefaultAsync(
                reception => reception.Id == id && reception.SalonId == salonId,
                cancellationToken);

    public Task<bool> AppointmentAlreadyLinkedAsync(
        long appointmentId,
        CancellationToken cancellationToken) =>
        context.Receptions.IgnoreQueryFilters().AnyAsync(
            item => item.AppointmentId == appointmentId,
            cancellationToken);

    public async Task<Result<T>> AddAsync<T>(
        ReceptionEntity reception,
        Func<ReceptionEntity, CancellationToken, Task<Result<T>>> transactionWork,
        CancellationToken cancellationToken)
    {
        var inventoryConnection = inventoryContext.Database.GetDbConnection();
        if (inventoryConnection.State != ConnectionState.Closed)
        {
            throw new InvalidOperationException(
                "Inventory context must not be used before joining a reception transaction.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);
        var inventoryTransactionJoined = false;
        try
        {
            inventoryContext.Database.SetDbConnection(
                context.Database.GetDbConnection(),
                contextOwnsConnection: false);
            await inventoryContext.Database.UseTransactionAsync(
                transaction.GetDbTransaction(),
                cancellationToken);
            inventoryTransactionJoined = true;
            reception.AssignReceptionNumber(
                await AllocateReceptionNumberAsync(reception.SalonId, transaction, cancellationToken));
            await context.Receptions.AddAsync(reception, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var result = await transactionWork(reception, cancellationToken);
            if (!result.IsSuccess)
            {
                await transaction.RollbackAsync(cancellationToken);
                context.ChangeTracker.Clear();
                inventoryContext.ChangeTracker.Clear();
                return result;
            }

            context.ReceptionPayments.AddRange(reception.Payments);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            context.ChangeTracker.Clear();
            inventoryContext.ChangeTracker.Clear();
            if (!reception.AppointmentId.HasValue)
                throw;

            var appointmentId = reception.AppointmentId.Value;
            await inventoryContext.Database.UseTransactionAsync(null, CancellationToken.None);
            inventoryTransactionJoined = false;
            await context.Database.UseTransactionAsync(null, CancellationToken.None);
            if (await AppointmentAlreadyLinkedAsync(appointmentId, cancellationToken))
                return Result.Conflict("این نوبت هم‌زمان به پذیرش دیگری متصل شده است.");
            throw;
        }
        finally
        {
            if (inventoryTransactionJoined)
            {
                await inventoryContext.Database.UseTransactionAsync(
                    null,
                    CancellationToken.None);
            }
            inventoryContext.Database.SetDbConnection(
                inventoryConnection,
                contextOwnsConnection: true);
        }
    }

    private async Task<long> AllocateReceptionNumberAsync(
        long salonId,
        IDbContextTransaction transaction,
        CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        command.Transaction = transaction.GetDbTransaction();
        command.CommandText = """
            DECLARE @Allocated TABLE (LastNumber bigint NOT NULL);

            MERGE [Reception].[ReceptionNumberSequences] WITH (HOLDLOCK) AS target
            USING (SELECT @SalonId AS [SalonId]) AS source
                ON target.[SalonId] = source.[SalonId]
            WHEN MATCHED THEN
                UPDATE SET [LastNumber] = target.[LastNumber] + 1
            WHEN NOT MATCHED THEN
                INSERT ([SalonId], [LastNumber]) VALUES (source.[SalonId], 1)
            OUTPUT inserted.[LastNumber] INTO @Allocated;

            SELECT [LastNumber] FROM @Allocated;
            """;

        var salonIdParameter = command.CreateParameter();
        salonIdParameter.ParameterName = "@SalonId";
        salonIdParameter.DbType = DbType.Int64;
        salonIdParameter.Value = salonId;
        command.Parameters.Add(salonIdParameter);

        var allocatedNumber = await command.ExecuteScalarAsync(cancellationToken);
        if (allocatedNumber is null or DBNull)
        {
            throw new InvalidOperationException("شماره پذیرش از پایگاه داده دریافت نشد.");
        }

        return Convert.ToInt64(allocatedNumber, CultureInfo.InvariantCulture);
    }

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
        catch (DbUpdateException)
        {
            var pendingPayment = context.ChangeTracker
                .Entries<ReceptionPayment>()
                .Where(entry => entry.State == EntityState.Added)
                .Select(entry => entry.Entity)
                .FirstOrDefault();
            if (pendingPayment is null)
            {
                throw;
            }

            var receptionId = pendingPayment.ReceptionId;
            var idempotencyKey = pendingPayment.IdempotencyKey;
            var transactionReference = pendingPayment.TransactionReference;
            context.ChangeTracker.Clear();
            var duplicate = await context.ReceptionPayments
                .IgnoreQueryFilters()
                .AnyAsync(payment =>
                    payment.ReceptionId == receptionId &&
                    (payment.IdempotencyKey == idempotencyKey ||
                     (transactionReference != null &&
                      payment.TransactionReference == transactionReference)),
                    cancellationToken);
            if (duplicate)
            {
                return false;
            }

            throw;
        }
    }

    public async Task<IReadOnlyList<CashRegisterPaymentOutboxMessage>> GetPendingAsync(
        int batchSize,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await context.CashRegisterPaymentOutbox
            .AsNoTracking()
            .Where(item => item.OutboxStatus == CashRegisterPaymentOutboxStatus.Pending &&
                           item.NextAttemptAt <= now)
            .OrderBy(item => item.Id)
            .Take(batchSize)
            .Select(item => new CashRegisterPaymentOutboxMessage(
                item.Id,
                item.SalonId,
                item.CashRegisterId,
                item.CashRegisterSessionId,
                item.ReceptionId,
                item.UserId,
                item.Amount,
                item.OccurredAt,
                item.IdempotencyKey,
                item.Description))
            .ToListAsync(cancellationToken);

    public async Task MarkProcessedAsync(
        long id,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        var item = await context.CashRegisterPaymentOutbox
            .SingleOrDefaultAsync(outbox => outbox.Id == id, cancellationToken);
        if (item is null || item.OutboxStatus == CashRegisterPaymentOutboxStatus.Processed)
            return;

        item.MarkProcessed(processedAt);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(
        long id,
        string error,
        DateTimeOffset attemptedAt,
        CancellationToken cancellationToken)
    {
        var item = await context.CashRegisterPaymentOutbox
            .SingleOrDefaultAsync(outbox => outbox.Id == id, cancellationToken);
        if (item is null || item.OutboxStatus == CashRegisterPaymentOutboxStatus.Processed)
            return;

        item.MarkFailed(error, attemptedAt);
        await context.SaveChangesAsync(cancellationToken);
    }

    async Task<IReadOnlyList<PersonnelCommissionOutboxMessage>>
        IPersonnelCommissionOutboxRepository.GetPendingAsync(
            int batchSize,
            DateTimeOffset now,
            CancellationToken cancellationToken) =>
        await context.PersonnelCommissionOutbox
            .AsNoTracking()
            .Where(item => item.OutboxStatus == PersonnelCommissionOutboxStatus.Pending &&
                           item.NextAttemptAt <= now)
            .OrderBy(item => item.Id)
            .Take(batchSize)
            .Select(item => new PersonnelCommissionOutboxMessage(
                item.Id,
                item.EventId,
                item.SalonId,
                item.ReceptionId,
                item.ReceptionServiceId))
            .ToListAsync(cancellationToken);

    async Task IPersonnelCommissionOutboxRepository.MarkProcessedAsync(
        long id,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken)
    {
        var item = await context.PersonnelCommissionOutbox
            .SingleOrDefaultAsync(outbox => outbox.Id == id, cancellationToken);
        if (item is null || item.OutboxStatus == PersonnelCommissionOutboxStatus.Processed)
            return;

        item.MarkProcessed(processedAt);
        await context.SaveChangesAsync(cancellationToken);
    }

    async Task IPersonnelCommissionOutboxRepository.MarkFailedAsync(
        long id,
        string error,
        DateTimeOffset attemptedAt,
        CancellationToken cancellationToken)
    {
        var item = await context.PersonnelCommissionOutbox
            .SingleOrDefaultAsync(outbox => outbox.Id == id, cancellationToken);
        if (item is null || item.OutboxStatus == PersonnelCommissionOutboxStatus.Processed)
            return;

        item.MarkFailed(error, attemptedAt);
        await context.SaveChangesAsync(cancellationToken);
    }
}
