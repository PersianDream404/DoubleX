using SharedKernel.Base;

namespace Modules.Reception.Domain.Entities;

public sealed class ReceptionServiceMaterial : BaseEntityIdentity
{
    public long ReceptionServiceId { get; private set; }
    public long MaterialId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal? UnitCost { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool MovementRegistered { get; private set; }
    public bool MovementReversed { get; private set; }
    public ReceptionService ReceptionService { get; private set; } = null!;

    private ReceptionServiceMaterial() { }

    private ReceptionServiceMaterial(
        long receptionServiceId,
        long materialId,
        decimal quantity,
        string idempotencyKey,
        string? description)
    {
        ReceptionServiceId = receptionServiceId;
        MaterialId = materialId;
        Quantity = quantity;
        IdempotencyKey = idempotencyKey;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    internal static ReceptionServiceMaterial Create(
        long receptionServiceId,
        long materialId,
        decimal quantity,
        string idempotencyKey,
        string? description) =>
        new(receptionServiceId, materialId, quantity, idempotencyKey, description);

    public void MarkMovementRegistered() => MovementRegistered = true;
    public void MarkMovementReversed() => MovementReversed = true;
}
