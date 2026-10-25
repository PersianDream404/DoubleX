using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Application.Contract.DTOs;

public sealed class RegisterStockMovementRequestDto
{
    public long MaterialId { get; set; }
    public StockMovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string? ReferenceType { get; set; }
    public long? ReferenceId { get; set; }
    public string? Description { get; set; }
    public string? IdempotencyKey { get; set; }
}
