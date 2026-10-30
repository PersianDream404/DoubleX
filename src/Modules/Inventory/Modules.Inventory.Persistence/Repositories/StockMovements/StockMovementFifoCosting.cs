using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;

namespace Modules.Inventory.Persistence.Repositories.StockMovements;

internal static class StockMovementFifoCosting
{
    public static List<Layer> BuildLayers(IEnumerable<Movement> history)
    {
        var layers = new List<Layer>();
        foreach (var movement in history)
        {
            switch (movement.Type)
            {
                case StockMovementType.Receipt:
                case StockMovementType.Return:
                case StockMovementType.AdjustmentIncrease:
                    var totalCost = movement.TotalCost ??
                        (movement.Type == StockMovementType.Receipt &&
                         movement.PurchaseQuantity.HasValue &&
                         movement.PurchaseUnitPrice.HasValue
                            ? decimal.Round(
                                movement.PurchaseQuantity.Value *
                                movement.PurchaseUnitPrice.Value,
                                2)
                            : null);
                    layers.Add(new Layer(movement.Quantity, totalCost));
                    break;
                case StockMovementType.Consumption:
                case StockMovementType.AdjustmentDecrease:
                    RemoveQuantity(layers, movement.Quantity);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported stock movement type: {movement.Type}.");
            }
        }

        return layers;
    }

    public static bool TryConsume(
        List<Layer> layers,
        decimal quantity,
        out decimal totalCost)
    {
        var remaining = quantity;
        totalCost = 0;
        var allocations = new List<(Layer Layer, decimal Quantity, decimal Cost)>();
        foreach (var layer in layers)
        {
            var consumed = Math.Min(remaining, layer.Quantity);
            if (consumed == 0)
            {
                continue;
            }

            if (!layer.TotalCost.HasValue)
            {
                totalCost = 0;
                return false;
            }

            var allocatedCost = consumed == layer.Quantity
                ? layer.TotalCost.Value
                : decimal.Round(
                    layer.TotalCost.Value * (consumed / layer.Quantity),
                    2);
            allocations.Add((layer, consumed, allocatedCost));
            totalCost += allocatedCost;
            remaining -= consumed;
            if (remaining == 0)
            {
                foreach (var allocation in allocations)
                {
                    allocation.Layer.Quantity -= allocation.Quantity;
                    allocation.Layer.TotalCost -= allocation.Cost;
                }

                return true;
            }
        }

        totalCost = 0;
        return false;
    }

    private static void RemoveQuantity(List<Layer> layers, decimal quantity)
    {
        while (quantity > 0 && layers.Count > 0)
        {
            var layer = layers[0];
            var consumed = Math.Min(quantity, layer.Quantity);
            if (layer.TotalCost.HasValue)
            {
                var allocatedCost = consumed == layer.Quantity
                    ? layer.TotalCost.Value
                    : decimal.Round(
                        layer.TotalCost.Value * (consumed / layer.Quantity),
                        2);
                layer.TotalCost -= allocatedCost;
            }

            quantity -= consumed;
            layer.Quantity -= consumed;
            if (layer.Quantity == 0)
            {
                layers.RemoveAt(0);
            }
        }

        if (quantity > 0)
        {
            throw new InvalidOperationException(
                "The stock movement ledger does not reconcile with the inventory balance.");
        }
    }

    internal sealed class Layer(decimal quantity, decimal? totalCost)
    {
        public decimal Quantity { get; set; } = quantity;
        public decimal? TotalCost { get; set; } = totalCost;
    }

    public sealed record Movement(
        StockMovementType Type,
        decimal Quantity,
        decimal? TotalCost,
        decimal? PurchaseQuantity,
        decimal? PurchaseUnitPrice);
}
