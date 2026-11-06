using Modules.Inventory.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation.Security;

public sealed class InventoryPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        InventoryPermissions.MaterialsList,
        InventoryPermissions.MaterialsDetails,
        InventoryPermissions.MaterialsCreate,
        InventoryPermissions.MaterialsUpdate,
        InventoryPermissions.MaterialsDelete,
        InventoryPermissions.UnitsList,
        InventoryPermissions.UnitsDetails,
        InventoryPermissions.UnitsCreate,
        InventoryPermissions.UnitsUpdate,
        InventoryPermissions.UnitsDelete,
        InventoryPermissions.StockMovementQuote,
        InventoryPermissions.StockMovementRegister
    ];
}
