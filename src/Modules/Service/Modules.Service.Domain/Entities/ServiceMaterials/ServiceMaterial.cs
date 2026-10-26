using SharedKernel.Base;

namespace Modules.Service.Domain.Entities;

public sealed class ServiceMaterial : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public long ServiceId { get; private set; }
    public long MaterialId { get; private set; }
    public decimal QuantityPerService { get; private set; }
    public string? Description { get; private set; }

    public Service Service { get; private set; } = null!;

    private ServiceMaterial() { }

    private ServiceMaterial(
        long salonId,
        long serviceId,
        long materialId,
        decimal quantityPerService,
        string? description)
    {
        SalonId = salonId;
        ServiceId = serviceId;
        MaterialId = materialId;
        QuantityPerService = quantityPerService;
        Description = Normalize(description);
    }

    public static ServiceMaterial Create(
        long salonId,
        long serviceId,
        long materialId,
        decimal quantityPerService,
        string? description) =>
        new(salonId, serviceId, materialId, quantityPerService, description);

    public void Update(decimal quantityPerService, string? description)
    {
        QuantityPerService = quantityPerService;
        Description = Normalize(description);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
