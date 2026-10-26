using SharedKernel.Base;

namespace Modules.Service.Domain.Entities;

public sealed class ServicePersonnel : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public long ServiceId { get; private set; }
    public long PersonnelId { get; private set; }

    public Service Service { get; private set; } = null!;

    private ServicePersonnel() { }

    private ServicePersonnel(long salonId, long serviceId, long personnelId)
    {
        SalonId = salonId;
        ServiceId = serviceId;
        PersonnelId = personnelId;
    }

    public static ServicePersonnel Create(long salonId, long serviceId, long personnelId) =>
        new(salonId, serviceId, personnelId);

    public void Update(long serviceId, long personnelId)
    {
        ServiceId = serviceId;
        PersonnelId = personnelId;
    }
}
