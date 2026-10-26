using SharedKernel.Base;

namespace Modules.Service.Domain.Entities;

public sealed class ServiceCategory : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive => Status;
    public ICollection<Service> Services { get; private set; } = new List<Service>();

    private ServiceCategory() { }

    private ServiceCategory(long salonId, string name, string? description)
    {
        SalonId = salonId;
        Name = name.Trim();
        Description = Normalize(description);
    }

    public static ServiceCategory Create(long salonId, string name, string? description) =>
        new(salonId, name, description);

    public void Update(string name, string? description, bool isActive)
    {
        Name = name.Trim();
        Description = Normalize(description);
        Status = isActive;
    }

    public void Deactivate() => Status = false;
    public void Activate() => Status = true;

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
