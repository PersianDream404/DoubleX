using SharedKernel.Base;

namespace Modules.Inventory.Domain.Entities;

public sealed class Unit : BaseEntityIdentity
{
    public string Name { get; private set; } = null!;
    public string Symbol { get; private set; } = null!;
    public UnitType UnitType { get; private set; }
    public bool IsActive => Status;

    public ICollection<Material> BaseUnitMaterials { get; private set; } = new List<Material>();
    public ICollection<Material> PurchaseUnitMaterials { get; private set; } = new List<Material>();

    private Unit()
    {
    }

    private Unit(string name, string symbol, UnitType unitType)
    {
        Name = name.Trim();
        Symbol = symbol.Trim();
        UnitType = unitType;
    }

    public static Unit Create(string name, string symbol, UnitType unitType) =>
        new(name, symbol, unitType);

    public void Update(string name, string symbol, UnitType unitType, bool isActive)
    {
        Name = name.Trim();
        Symbol = symbol.Trim();
        UnitType = unitType;
        Status = isActive;
    }

    public void Deactivate() => Status = false;

    public void Activate() => Status = true;
}
