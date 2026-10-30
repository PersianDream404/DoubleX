using Microsoft.EntityFrameworkCore;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.Interfaces.StockMovements;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Persistence.Context;

namespace Modules.Inventory.Persistence.Repositories.StockMovements;

public sealed class StockMovementCostQueryRepository(InventoryReadDbContext context)
    : IStockMovementCostQueryRepository
{
    public async Task<StockMovementCostQuoteResult> QuoteAsync(
        QuoteStockMovementCostsRequestDto request,
        CancellationToken cancellationToken)
    {
        var materialIds = request.Items
            .Select(item => item.MaterialId)
            .Distinct()
            .ToArray();
        var materials = await context.Materials
            .AsNoTracking()
            .Where(item => materialIds.Contains(item.Id))
            .Select(item => new
            {
                item.Id,
                item.Status,
                StockQuantity = item.StockBalance == null ? 0m : item.StockBalance.Quantity
            })
            .ToListAsync(cancellationToken);

        if (materials.Count != materialIds.Length)
        {
            return new StockMovementCostQuoteResult(InventoryOperationStatus.NotFound);
        }

        if (materials.Any(item => !item.Status))
        {
            return new StockMovementCostQuoteResult(InventoryOperationStatus.Inactive);
        }

        foreach (var group in request.Items.GroupBy(item => item.MaterialId))
        {
            var availableStock = materials.Single(item => item.Id == group.Key).StockQuantity;
            if (group.Sum(item => item.Quantity) > availableStock)
            {
                return new StockMovementCostQuoteResult(InventoryOperationStatus.InsufficientStock);
            }
        }

        var history = await context.StockMovements
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(item => materialIds.Contains(item.MaterialId))
            .OrderBy(item => item.MaterialId)
            .ThenBy(item => item.Id)
            .Select(item => new
            {
                item.MaterialId,
                Movement = new StockMovementFifoCosting.Movement(
                    item.Type,
                    item.Quantity,
                    item.TotalCost,
                    item.PurchaseQuantity,
                    item.PurchaseUnitPrice)
            })
            .ToListAsync(cancellationToken);
        var layersByMaterial = history
            .GroupBy(item => item.MaterialId)
            .ToDictionary(
                group => group.Key,
                group => StockMovementFifoCosting.BuildLayers(
                    group.Select(item => item.Movement)));

        var quoteItems = new List<QuoteStockMovementCostResultItemDto>(request.Items.Count);
        foreach (var item in request.Items)
        {
            if (!layersByMaterial.TryGetValue(item.MaterialId, out var layers) ||
                !StockMovementFifoCosting.TryConsume(layers, item.Quantity, out var totalCost))
            {
                return new StockMovementCostQuoteResult(
                    InventoryOperationStatus.HistoricalDataConflict);
            }

            quoteItems.Add(new QuoteStockMovementCostResultItemDto(
                item.Key,
                decimal.Round(totalCost / item.Quantity, 8),
                totalCost));
        }

        return new StockMovementCostQuoteResult(
            InventoryOperationStatus.Succeeded,
            new QuoteStockMovementCostsResponseDto(
                quoteItems,
                quoteItems.Sum(item => item.TotalCost)));
    }
}
