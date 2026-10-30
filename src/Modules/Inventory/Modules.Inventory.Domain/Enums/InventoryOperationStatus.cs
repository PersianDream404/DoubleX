namespace Modules.Inventory.Domain.Enums;

public enum InventoryOperationStatus
{
    Succeeded,
    NotFound,
    Inactive,
    Conflict,
    HistoricalDataConflict,
    AmountLimitExceeded,
    InvalidInput,
    InsufficientStock,
    ConcurrencyConflict
}
