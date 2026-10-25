using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;

namespace Modules.Inventory.Domain.Interfaces.Units;

public interface IUnitCommandRepository
{
    Task<InventoryOperationStatus> AddAsync(Unit unit, CancellationToken cancellationToken);
    Task<InventoryOperationStatus> UpdateAsync(Unit unit, CancellationToken cancellationToken);
    Task<InventoryOperationStatus> DeleteAsync(long id, CancellationToken cancellationToken);
    Task<InventoryOperationStatus> DeactivateAsync(long id, CancellationToken cancellationToken);
}
