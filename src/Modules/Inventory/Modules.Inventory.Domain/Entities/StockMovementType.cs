namespace Modules.Inventory.Domain.Entities;

public enum StockMovementType
{
    Receipt = 1,
    Consumption = 2,
    Return = 3,
    AdjustmentIncrease = 4,
    AdjustmentDecrease = 5
}
