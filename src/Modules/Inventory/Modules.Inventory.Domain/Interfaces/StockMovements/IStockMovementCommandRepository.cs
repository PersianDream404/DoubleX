using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;

namespace Modules.Inventory.Domain.Interfaces.StockMovements;

public interface IStockMovementCommandRepository
{
    Task<StockMovementRegistrationResult> RegisterAsync(
        StockMovement movement,
        decimal? maximumTotalCost,
        CancellationToken cancellationToken);
}

public sealed record StockMovementRegistrationResult(
    InventoryOperationStatus Status,
    decimal? UnitCost = null,
    decimal? TotalCost = null);
