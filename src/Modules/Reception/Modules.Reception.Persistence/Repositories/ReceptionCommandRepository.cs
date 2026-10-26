using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Domain.Entities;
using Modules.Reception.Persistence.Context;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Persistence.Repositories;

public sealed class ReceptionCommandRepository(ReceptionWriteDbContext context)
    : IReceptionCommandRepository
{
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

    public async Task AddAsync(ReceptionEntity reception, CancellationToken cancellationToken)
    {
        await context.Receptions.AddAsync(reception, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
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
}
