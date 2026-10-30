using SharedKernel.Base;

namespace Modules.Inventory.Domain.Entities;

public sealed class StockMovement : BaseEntityIdentity
{
    public long MaterialId { get; private set; }
    public StockMovementType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal? PurchaseQuantity { get; private set; }
    public decimal? PurchaseUnitPrice { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? ReferenceType { get; private set; }
    public long? ReferenceId { get; private set; }
    public string? Description { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public Material Material { get; private set; } = null!;

    private StockMovement()
    {
    }

    private StockMovement(
        long materialId,
        StockMovementType type,
        decimal quantity,
        decimal? purchaseQuantity,
        decimal? purchaseUnitPrice,
        DateTimeOffset occurredAt,
        string? referenceType,
        long? referenceId,
        string? description,
        string? idempotencyKey)
    {
        MaterialId = materialId;
        Type = type;
        Quantity = quantity;
        PurchaseQuantity = purchaseQuantity;
        PurchaseUnitPrice = purchaseUnitPrice;
        OccurredAt = occurredAt;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
        Description = description;
        IdempotencyKey = idempotencyKey;
    }

    public static StockMovement Create(
        long materialId,
        StockMovementType type,
        decimal quantity,
        decimal? purchaseQuantity,
        decimal? purchaseUnitPrice,
        DateTimeOffset occurredAt,
        string? referenceType,
        long? referenceId,
        string? description,
        string? idempotencyKey) =>
        new(
            materialId,
            type,
            quantity,
            purchaseQuantity,
            purchaseUnitPrice,
            occurredAt,
            referenceType,
            referenceId,
            description,
            idempotencyKey);

    public decimal GetQuantityChange() => Type switch
    {
        StockMovementType.Receipt or StockMovementType.Return or StockMovementType.AdjustmentIncrease => Quantity,
        StockMovementType.Consumption or StockMovementType.AdjustmentDecrease => -Quantity,
        _ => throw new InvalidOperationException($"Unsupported stock movement type: {Type}.")
    };
}
