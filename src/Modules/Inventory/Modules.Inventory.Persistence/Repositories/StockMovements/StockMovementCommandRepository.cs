using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.StockMovements;
using Modules.Inventory.Persistence.Context;

namespace Modules.Inventory.Persistence.Repositories.StockMovements;

public sealed class StockMovementCommandRepository(InventoryWriteDbContext context)
    : IStockMovementCommandRepository
{
    public async Task<InventoryOperationStatus> RegisterAsync(
        StockMovement movement,
        CancellationToken cancellationToken)
    {
        if (movement.Type == StockMovementType.Consumption &&
            string.IsNullOrWhiteSpace(movement.IdempotencyKey))
        {
            return InventoryOperationStatus.InvalidInput;
        }

        if (movement.IdempotencyKey is not null &&
            await context.StockMovements.AnyAsync(
                existing => existing.IdempotencyKey == movement.IdempotencyKey,
                cancellationToken))
        {
            return InventoryOperationStatus.Conflict;
        }

        var material = await context.Materials
            .SingleOrDefaultAsync(item => item.Id == movement.MaterialId, cancellationToken);
        if (material is null)
        {
            return InventoryOperationStatus.NotFound;
        }

        if (!material.Status)
        {
            return InventoryOperationStatus.Inactive;
        }

        var balance = await context.StockBalances
            .SingleOrDefaultAsync(item => item.MaterialId == movement.MaterialId, cancellationToken);
        if (balance is null)
        {
            throw new InvalidOperationException(
                $"Material {movement.MaterialId} has no stock balance, violating the inventory invariant.");
        }

        if (!balance.TryApply(movement.GetQuantityChange()))
        {
            return InventoryOperationStatus.InsufficientStock;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await context.StockMovements.AddAsync(movement, cancellationToken);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return InventoryOperationStatus.Succeeded;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return InventoryOperationStatus.ConcurrencyConflict;
        }
        catch (DbUpdateException) when (movement.IdempotencyKey is not null)
        {
            await transaction.RollbackAsync(cancellationToken);
            context.ChangeTracker.Clear();
            var keyAlreadyUsed = await context.StockMovements
                .IgnoreQueryFilters()
                .AnyAsync(
                    existing => existing.IdempotencyKey == movement.IdempotencyKey,
                    cancellationToken);
            if (keyAlreadyUsed)
            {
                return InventoryOperationStatus.Conflict;
            }

            throw;
        }
    }
}
