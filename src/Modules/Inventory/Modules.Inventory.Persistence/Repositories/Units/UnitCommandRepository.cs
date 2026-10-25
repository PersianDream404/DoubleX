using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Units;
using Modules.Inventory.Persistence.Context;

namespace Modules.Inventory.Persistence.Repositories.Units;

public sealed class UnitCommandRepository(InventoryWriteDbContext context)
    : IUnitCommandRepository
{
    public async Task<InventoryOperationStatus> AddAsync(
        Unit unit,
        CancellationToken cancellationToken)
    {
        var alreadyExists = await context.Units.IgnoreQueryFilters().AnyAsync(existing =>
                existing.Name == unit.Name || existing.Symbol == unit.Symbol,
            cancellationToken);
        if (alreadyExists)
        {
            return InventoryOperationStatus.Conflict;
        }

        await context.Units.AddAsync(unit, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }

    public async Task<InventoryOperationStatus> UpdateAsync(
        Unit unit,
        CancellationToken cancellationToken)
    {
        var existing = await context.Units.SingleOrDefaultAsync(
            item => item.Id == unit.Id,
            cancellationToken);
        if (existing is null)
        {
            return InventoryOperationStatus.NotFound;
        }

        var duplicate = await context.Units.IgnoreQueryFilters().AnyAsync(
            item => item.Id != unit.Id &&
                    (item.Name == unit.Name || item.Symbol == unit.Symbol),
            cancellationToken);
        if (duplicate)
        {
            return InventoryOperationStatus.Conflict;
        }

        existing.Update(unit.Name, unit.Symbol, unit.UnitType, unit.Status);
        await context.SaveChangesAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }

    public async Task<InventoryOperationStatus> DeleteAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var unit = await context.Units.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (unit is null)
        {
            return InventoryOperationStatus.NotFound;
        }

        var isUsed = await context.Materials.AnyAsync(
            material => material.BaseUnitId == id || material.PurchaseUnitId == id,
            cancellationToken);
        if (isUsed)
        {
            return InventoryOperationStatus.Conflict;
        }

        unit.Deleted = true;
        await context.SaveChangesAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }

    public async Task<InventoryOperationStatus> DeactivateAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var unit = await context.Units.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (unit is null)
        {
            return InventoryOperationStatus.NotFound;
        }

        unit.Deactivate();
        await context.SaveChangesAsync(cancellationToken);
        return InventoryOperationStatus.Succeeded;
    }
}
