using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Domain.Enums;

namespace Modules.Inventory.Application.Contract.Interfaces.StockMovements;

public interface IStockMovementCostQueryRepository
{
    Task<StockMovementCostQuoteResult> QuoteAsync(
        QuoteStockMovementCostsRequestDto request,
        CancellationToken cancellationToken);
}

public sealed record StockMovementCostQuoteResult(
    InventoryOperationStatus Status,
    QuoteStockMovementCostsResponseDto? Quote = null);
