using SharedKernel.Base;

namespace Modules.Reception.Domain.Entities;

public sealed class ReceptionService : BaseEntityIdentity
{
    private readonly List<ReceptionServiceMaterial> _materials = [];

    public long ReceptionId { get; private set; }
    public long ServiceId { get; private set; }
    public long ServiceTariffId { get; private set; }
    public long? PersonnelId { get; private set; }
    public string ServiceName { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public ReceptionServiceStatus ServiceStatus { get; private set; }
    public IReadOnlyCollection<ReceptionServiceMaterial> Materials => _materials;
    public Reception Reception { get; private set; } = null!;

    private ReceptionService() { }

    private ReceptionService(
        long receptionId,
        long serviceId,
        long tariffId,
        string serviceName,
        long? personnelId,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount)
    {
        ReceptionId = receptionId;
        ServiceId = serviceId;
        ServiceTariffId = tariffId;
        PersonnelId = personnelId;
        ServiceName = serviceName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountAmount = discountAmount;
        FinalAmount = (quantity * unitPrice) - discountAmount;
        ServiceStatus = ReceptionServiceStatus.Pending;
    }

    internal static ReceptionService Create(
        long receptionId,
        long serviceId,
        long tariffId,
        string serviceName,
        long? personnelId,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount) =>
        new(receptionId, serviceId, tariffId, serviceName, personnelId, quantity, unitPrice, discountAmount);

    public bool TryAddMaterial(long materialId, decimal quantity, string idempotencyKey, string? description)
    {
        if (ServiceStatus == ReceptionServiceStatus.Cancelled ||
            materialId <= 0 ||
            quantity <= 0 ||
            _materials.Any(material => material.IdempotencyKey == idempotencyKey))
        {
            return false;
        }

        _materials.Add(ReceptionServiceMaterial.Create(
            Id,
            materialId,
            quantity,
            idempotencyKey,
            description));
        return true;
    }

    public decimal? GetMaterialCharge(string idempotencyKey, decimal totalCost)
    {
        var material = _materials.SingleOrDefault(item => item.IdempotencyKey == idempotencyKey);
        return material is null || material.MovementRegistered
            ? null
            : decimal.Round(totalCost, 2);
    }

    public bool MarkMaterialMovementRegistered(
        string idempotencyKey,
        decimal unitCost,
        decimal totalCost)
    {
        var material = _materials.SingleOrDefault(item => item.IdempotencyKey == idempotencyKey);
        if (material is null || material.MovementRegistered)
        {
            return false;
        }

        material.MarkMovementRegistered(unitCost, totalCost);
        return true;
    }

    public bool MarkMaterialMovementReversed(long materialRecordId)
    {
        var material = _materials.SingleOrDefault(item => item.Id == materialRecordId);
        if (material is null || !material.MovementRegistered || material.MovementReversed)
        {
            return false;
        }

        material.MarkMovementReversed();
        return true;
    }

    public bool ChangeStatus(ReceptionServiceStatus status)
    {
        if (ServiceStatus is ReceptionServiceStatus.Completed or ReceptionServiceStatus.Cancelled ||
            status is not (ReceptionServiceStatus.InProgress or ReceptionServiceStatus.Completed or ReceptionServiceStatus.Cancelled))
        {
            return false;
        }

        if (status == ReceptionServiceStatus.Cancelled &&
            _materials.Any(material => !material.MovementRegistered || !material.MovementReversed))
        {
            return false;
        }

        ServiceStatus = status;
        return true;
    }
}
