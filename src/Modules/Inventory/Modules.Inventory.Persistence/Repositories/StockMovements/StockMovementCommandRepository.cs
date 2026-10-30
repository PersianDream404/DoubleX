using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.StockMovements;
using Modules.Inventory.Persistence.Context;

namespace Modules.Inventory.Persistence.Repositories.StockMovements;

public sealed class StockMovementCommandRepository(InventoryWriteDbContext context)
    : IStockMovementCommandRepository
{
    public async Task<StockMovementRegistrationResult> RegisterAsync(
        StockMovement movement,
        decimal? maximumTotalCost,
        CancellationToken cancellationToken)
    {
        if (movement.Type is StockMovementType.Consumption or StockMovementType.Return &&
            string.IsNullOrWhiteSpace(movement.IdempotencyKey))
        {
            return new StockMovementRegistrationResult(InventoryOperationStatus.InvalidInput);
        }

        if (movement.IdempotencyKey is not null)
        {
            var existingMovement = await context.StockMovements
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    existing => existing.IdempotencyKey == movement.IdempotencyKey,
                    cancellationToken);
            if (existingMovement is not null)
            {
                return GetExistingResult(existingMovement, movement, maximumTotalCost);
            }
        }

        var material = await context.Materials
            .SingleOrDefaultAsync(item => item.Id == movement.MaterialId, cancellationToken);
        if (material is null)
        {
            return new StockMovementRegistrationResult(InventoryOperationStatus.NotFound);
        }

        if (!material.Status)
        {
            return new StockMovementRegistrationResult(InventoryOperationStatus.Inactive);
        }

        if (movement.Type == StockMovementType.Receipt &&
            (movement.PurchaseQuantity is null ||
             movement.PurchaseUnitPrice is null ||
             decimal.Round(
                 movement.PurchaseQuantity.Value * material.PurchaseConversionFactor,
                 3) != movement.Quantity))
        {
            return new StockMovementRegistrationResult(InventoryOperationStatus.InvalidInput);
        }

        if (movement.Type == StockMovementType.Receipt)
        {
            movement.SetTotalCost(
                movement.PurchaseQuantity!.Value *
                movement.PurchaseUnitPrice!.Value);
            movement.SetUnitCost(
                movement.TotalCost!.Value / movement.Quantity);
        }

        var balance = await context.StockBalances
            .SingleOrDefaultAsync(item => item.MaterialId == movement.MaterialId, cancellationToken);
        if (balance is null)
        {
            throw new InvalidOperationException(
                $"Material {movement.MaterialId} has no stock balance, violating the inventory invariant.");
        }

        if (balance.Quantity + movement.GetQuantityChange() < 0)
        {
            return new StockMovementRegistrationResult(InventoryOperationStatus.InsufficientStock);
        }

        var history = await context.StockMovements
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(item => item.MaterialId == movement.MaterialId)
            .OrderBy(item => item.Id)
            .Select(item => new StockMovementFifoCosting.Movement(
                item.Type,
                item.Quantity,
                item.TotalCost,
                item.PurchaseQuantity,
                item.PurchaseUnitPrice))
            .ToListAsync(cancellationToken);

        if (movement.Type == StockMovementType.Return)
        {
            var existingReturn = await context.StockMovements
                .IgnoreQueryFilters()
                .AsNoTracking()
                .AnyAsync(item =>
                    item.Type == StockMovementType.Return &&
                    item.ReferenceType == movement.ReferenceType &&
                    item.ReferenceId == movement.ReferenceId,
                    cancellationToken);
            if (existingReturn)
            {
                return new StockMovementRegistrationResult(InventoryOperationStatus.Conflict);
            }

            var originalConsumption = await context.StockMovements
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(item =>
                    item.Type == StockMovementType.Consumption &&
                    item.ReferenceType == movement.ReferenceType &&
                    item.ReferenceId == movement.ReferenceId)
                .Select(item => new { item.TotalCost })
                .SingleOrDefaultAsync(cancellationToken);
            if (originalConsumption?.TotalCost is null)
            {
                return new StockMovementRegistrationResult(
                    InventoryOperationStatus.HistoricalDataConflict);
            }

            movement.SetTotalCost(originalConsumption.TotalCost.Value);
            movement.SetUnitCost(originalConsumption.TotalCost.Value / movement.Quantity);
        }

        var layers = StockMovementFifoCosting.BuildLayers(history);
        if (movement.Type == StockMovementType.Consumption)
        {
            if (!StockMovementFifoCosting.TryConsume(layers, movement.Quantity, out var totalCost))
            {
                return new StockMovementRegistrationResult(
                    InventoryOperationStatus.HistoricalDataConflict);
            }

            movement.SetTotalCost(totalCost);
            movement.SetUnitCost(totalCost / movement.Quantity);
            if (maximumTotalCost.HasValue && totalCost > maximumTotalCost.Value)
            {
                return new StockMovementRegistrationResult(
                    InventoryOperationStatus.AmountLimitExceeded);
            }
        }

        if (!balance.TryApply(movement.GetQuantityChange()))
        {
            return new StockMovementRegistrationResult(InventoryOperationStatus.InsufficientStock);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await context.StockMovements.AddAsync(movement, cancellationToken);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new StockMovementRegistrationResult(
                InventoryOperationStatus.Succeeded,
                movement.UnitCost,
                movement.TotalCost);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new StockMovementRegistrationResult(InventoryOperationStatus.ConcurrencyConflict);
        }
        catch (DbUpdateException) when (movement.IdempotencyKey is not null)
        {
            await transaction.RollbackAsync(cancellationToken);
            context.ChangeTracker.Clear();
            var existingMovement = await context.StockMovements
                .IgnoreQueryFilters()
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    existing => existing.IdempotencyKey == movement.IdempotencyKey,
                    cancellationToken);
            if (existingMovement is not null)
            {
                return GetExistingResult(existingMovement, movement, maximumTotalCost);
            }

            throw;
        }
    }

    private static bool Matches(StockMovement existing, StockMovement requested) =>
        existing.MaterialId == requested.MaterialId &&
        existing.Type == requested.Type &&
        existing.Quantity == requested.Quantity &&
        existing.PurchaseQuantity == requested.PurchaseQuantity &&
        existing.PurchaseUnitPrice == requested.PurchaseUnitPrice &&
        existing.ReferenceType == requested.ReferenceType &&
        existing.ReferenceId == requested.ReferenceId;

    private static StockMovementRegistrationResult GetExistingResult(
        StockMovement existing,
        StockMovement requested,
        decimal? maximumTotalCost)
    {
        if (!Matches(existing, requested))
        {
            return new StockMovementRegistrationResult(InventoryOperationStatus.Conflict);
        }

        if (existing.Type is StockMovementType.Consumption or StockMovementType.Return &&
            existing.TotalCost is null)
        {
            return new StockMovementRegistrationResult(
                InventoryOperationStatus.HistoricalDataConflict);
        }
        if (maximumTotalCost.HasValue &&
            existing.Type == StockMovementType.Consumption &&
            existing.TotalCost.HasValue &&
            existing.TotalCost.Value > maximumTotalCost.Value)
        {
            return new StockMovementRegistrationResult(
                InventoryOperationStatus.AmountLimitExceeded);
        }

        return new StockMovementRegistrationResult(
            InventoryOperationStatus.Succeeded,
            existing.UnitCost,
            existing.TotalCost);
    }

}
