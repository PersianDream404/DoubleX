using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;

namespace Modules.Inventory.Domain.Interfaces.Materials;

public interface IMaterialCommandRepository
{
    Task<InventoryOperationStatus> AddAsync(Material material, CancellationToken cancellationToken);
    Task<InventoryOperationStatus> UpdateAsync(Material material, CancellationToken cancellationToken);
    Task<InventoryOperationStatus> DeleteAsync(long id, CancellationToken cancellationToken);
    Task<InventoryOperationStatus> DeactivateAsync(long id, CancellationToken cancellationToken);
}
