namespace Modules.Inventory.Application.Contract.Security;

public static class InventoryPermissions
{
    public const string MaterialsList = "Inventory.Materials.List";
    public const string MaterialsDetails = "Inventory.Materials.Details";
    public const string MaterialsCreate = "Inventory.Materials.Create";
    public const string MaterialsUpdate = "Inventory.Materials.Update";
    public const string MaterialsDelete = "Inventory.Materials.Delete";
    public const string UnitsList = "Inventory.Units.List";
    public const string UnitsDetails = "Inventory.Units.Details";
    public const string UnitsCreate = "Inventory.Units.Create";
    public const string UnitsUpdate = "Inventory.Units.Update";
    public const string UnitsDelete = "Inventory.Units.Delete";
    public const string StockMovementQuote = "Inventory.StockMovement.Quote";
    public const string StockMovementRegister = "Inventory.StockMovement.Register";
}
