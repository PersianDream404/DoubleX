using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Materials;
using Modules.Inventory.Persistence.Context;

namespace Modules.Inventory.Persistence.Repositories.Materials;

public sealed class MaterialCommandRepository(InventoryWriteDbContext context)
    : IMaterialCommandRepository
{
    public async Task<InventoryOperationStatus> AddAsync(
        Material material,
        CancellationToken cancellationToken)
    {
        var duplicateCode = await context.Materials.IgnoreQueryFilters().AnyAsync(
            existing => existing.Code == material.Code,
            cancellationToken);
        if (duplicateCode)
        {
            return InventoryOperationStatus.Conflict;
        }

        var units = await context.Units
            .Where(unit => unit.Id == material.BaseUnitId || unit.Id == material.PurchaseUnitId)
            .ToListAsync(cancellationToken);
        if (units.Count != (material.BaseUnitId == material.PurchaseUnitId ? 1 : 2))
        {
            return InventoryOperationStatus.NotFound;
        }

        if (units.Any(unit => !unit.Status))
        {
            return InventoryOperationStatus.Inactive;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await context.Materials.AddAsync(material, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await context.StockBalances.AddAsync(StockBalance.Create(material.Id), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }

    public async Task<InventoryOperationStatus> UpdateAsync(
        Material material,
        CancellationToken cancellationToken)
    {
        var existing = await context.Materials.SingleOrDefaultAsync(
            item => item.Id == material.Id,
            cancellationToken);
        if (existing is null)
        {
            return InventoryOperationStatus.NotFound;
        }

        var duplicateCode = await context.Materials.IgnoreQueryFilters().AnyAsync(
            item => item.Id != material.Id && item.Code == material.Code,
            cancellationToken);
        if (duplicateCode)
        {
            return InventoryOperationStatus.Conflict;
        }

        if (existing.BaseUnitId != material.BaseUnitId)
        {
            var hasMovements = await context.StockMovements.AnyAsync(
                movement => movement.MaterialId == material.Id,
                cancellationToken);
            var balance = await context.StockBalances.SingleOrDefaultAsync(
                item => item.MaterialId == material.Id,
                cancellationToken);
            if (hasMovements || balance?.Quantity > 0)
            {
                return InventoryOperationStatus.HistoricalDataConflict;
            }
        }

        var units = await context.Units
            .Where(unit => unit.Id == material.BaseUnitId || unit.Id == material.PurchaseUnitId)
            .ToListAsync(cancellationToken);
        if (units.Count != (material.BaseUnitId == material.PurchaseUnitId ? 1 : 2))
        {
            return InventoryOperationStatus.NotFound;
        }

        if (units.Any(unit => !unit.Status))
        {
            return InventoryOperationStatus.Inactive;
        }

        existing.Update(
            material.Name,
            material.Code,
            material.Brand,
            material.BaseUnitId,
            material.PurchaseUnitId,
            material.PurchaseConversionFactor,
            material.MinimumStock,
            material.Status);
        await context.SaveChangesAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }

    public async Task<InventoryOperationStatus> DeleteAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var material = await context.Materials.SingleOrDefaultAsync(
            item => item.Id == id,
            cancellationToken);
        if (material is null)
        {
            return InventoryOperationStatus.NotFound;
        }

        material.Deleted = true;
        await context.SaveChangesAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }

    public async Task<InventoryOperationStatus> DeactivateAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var material = await context.Materials.SingleOrDefaultAsync(
            item => item.Id == id,
            cancellationToken);
        if (material is null)
        {
            return InventoryOperationStatus.NotFound;
        }

        material.Deactivate();
        await context.SaveChangesAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }
}
