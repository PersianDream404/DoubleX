using SharedKernel.Base;

namespace Modules.Service.Domain.Entities;

public sealed class ServicePrice : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public long ServiceId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ValidTo { get; private set; }

    public Service Service { get; private set; } = null!;

    private ServicePrice() { }

    private ServicePrice(
        long salonId,
        long serviceId,
        decimal amount,
        DateTimeOffset validFrom,
        DateTimeOffset? validTo)
    {
        SalonId = salonId;
        ServiceId = serviceId;
        Amount = amount;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public static ServicePrice Create(
        long salonId,
        long serviceId,
        decimal amount,
        DateTimeOffset validFrom,
        DateTimeOffset? validTo) =>
        new(salonId, serviceId, amount, validFrom, validTo);

    public void CloseAt(DateTimeOffset validTo)
    {
        if (ValidTo.HasValue || validTo <= ValidFrom)
            throw new InvalidOperationException("The service price validity interval cannot be closed at this time.");
        ValidTo = validTo;
    }
}
