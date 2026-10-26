using SharedKernel.Base;

namespace Modules.Service.Domain.Entities;

public sealed class Service : BaseEntityIdentity
{
    public long SalonId { get; private set; }
    public long CategoryId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public bool IsActive => Status;

    public ServiceCategory Category { get; private set; } = null!;
    public ICollection<ServicePrice> Prices { get; private set; } = new List<ServicePrice>();
    public ICollection<ServiceMaterial> Materials { get; private set; } = new List<ServiceMaterial>();

    private Service() { }

    private Service(
        long salonId,
        long categoryId,
        string code,
        string name,
        string? description,
        int durationMinutes)
    {
        SalonId = salonId;
        CategoryId = categoryId;
        Code = code.Trim();
        Name = name.Trim();
        Description = Normalize(description);
        DurationMinutes = durationMinutes;
    }

    public static Service Create(
        long salonId,
        long categoryId,
        string code,
        string name,
        string? description,
        int durationMinutes) =>
        new(salonId, categoryId, code, name, description, durationMinutes);

    public void Update(
        long categoryId,
        string code,
        string name,
        string? description,
        int durationMinutes,
        bool isActive)
    {
        CategoryId = categoryId;
        Code = code.Trim();
        Name = name.Trim();
        Description = Normalize(description);
        DurationMinutes = durationMinutes;
        Status = isActive;
    }

    public void Deactivate() => Status = false;
    public void Activate() => Status = true;

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
