using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;

namespace Modules.Inventory.Domain.Interfaces.StockMovements;

public interface IStockMovementCommandRepository
{
    Task<InventoryOperationStatus> RegisterAsync(
        StockMovement movement,
        CancellationToken cancellationToken);
}
