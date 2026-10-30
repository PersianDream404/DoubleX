namespace Modules.Inventory.Application.Contract.DTOs;

public sealed class QuoteStockMovementCostsRequestDto
{
    public List<QuoteStockMovementCostItemDto> Items { get; set; } = [];
}

public sealed class QuoteStockMovementCostItemDto
{
    public string Key { get; set; } = null!;
    public long MaterialId { get; set; }
    public decimal Quantity { get; set; }
}

public sealed record QuoteStockMovementCostResultItemDto(
    string Key,
    decimal UnitCost,
    decimal TotalCost);

public sealed record QuoteStockMovementCostsResponseDto(
    IReadOnlyList<QuoteStockMovementCostResultItemDto> Items,
    decimal TotalCost);
