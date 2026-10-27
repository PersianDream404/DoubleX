using SharedKernel.Base;

namespace Modules.CashRegister.Domain.Entities;

public sealed class CashRegister : BaseEntityIdentity
{
    private readonly List<CashRegisterSession> _sessions = [];

    private CashRegister() { }

    private CashRegister(long salonId, string name, string code, string? description)
    {
        SalonId = salonId;
        Rename(name, code, description);
        Status = true;
    }

    public long SalonId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public IReadOnlyCollection<CashRegisterSession> Sessions => _sessions;

    public static CashRegister Create(long salonId, string name, string code, string? description) =>
        new(salonId, name, code, description);

    public void Rename(string name, string code, string? description)
    {
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void SetActive(bool active) => Status = active;
}
