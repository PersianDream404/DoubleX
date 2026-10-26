using SharedKernel.Base;

namespace Modules.Service.Domain.Entities;

public sealed class ServiceTariffItem : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public long TariffId { get; private set; }
    public long ServiceId { get; private set; }
    public decimal Amount { get; private set; }

    public ServiceTariff Tariff { get; private set; } = null!;
    public Service Service { get; private set; } = null!;

    private ServiceTariffItem() { }

    private ServiceTariffItem(long salonId, long tariffId, long serviceId, decimal amount)
    {
        SalonId = salonId;
        TariffId = tariffId;
        ServiceId = serviceId;
        Amount = amount;
    }

    public static ServiceTariffItem Create(long salonId, long tariffId, long serviceId, decimal amount) =>
        new(salonId, tariffId, serviceId, amount);
}
