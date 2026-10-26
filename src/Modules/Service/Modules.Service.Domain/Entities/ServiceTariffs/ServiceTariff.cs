using SharedKernel.Base;

namespace Modules.Service.Domain.Entities;

public sealed class ServiceTariff : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ValidTo { get; private set; }
    public ICollection<ServiceTariffItem> Items { get; private set; } = new List<ServiceTariffItem>();

    private ServiceTariff() { }

    private ServiceTariff(
        long salonId,
        string name,
        DateTimeOffset validFrom,
        DateTimeOffset? validTo)
    {
        if (validTo.HasValue && validTo.Value <= validFrom)
            throw new ArgumentException("Tariff validity end must be after its start.", nameof(validTo));

        SalonId = salonId;
        Name = name.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public static ServiceTariff Create(
        long salonId,
        string name,
        DateTimeOffset validFrom,
        DateTimeOffset? validTo) =>
        new(salonId, name, validFrom, validTo);
}
