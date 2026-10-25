using SharedKernel.Base;

namespace Modules.Inventory.Domain.Entities;

public sealed class Material : BaseEntityIdentity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Brand { get; private set; }
    public long BaseUnitId { get; private set; }
    public long PurchaseUnitId { get; private set; }
    public decimal PurchaseConversionFactor { get; private set; }
    public decimal MinimumStock { get; private set; }
    public bool IsActive => Status;

    public Unit BaseUnit { get; private set; } = null!;
    public Unit PurchaseUnit { get; private set; } = null!;
    public StockBalance? StockBalance { get; private set; }
    public ICollection<StockMovement> StockMovements { get; private set; } = new List<StockMovement>();

    private Material()
    {
    }

    private Material(
        string name,
        string code,
        string? brand,
        long baseUnitId,
        long purchaseUnitId,
        decimal purchaseConversionFactor,
        decimal minimumStock)
    {
        Name = name.Trim();
        Code = code.Trim();
        Brand = string.IsNullOrWhiteSpace(brand) ? null : brand.Trim();
        BaseUnitId = baseUnitId;
        PurchaseUnitId = purchaseUnitId;
        PurchaseConversionFactor = purchaseConversionFactor;
        MinimumStock = minimumStock;
    }

    public static Material Create(
        string name,
        string code,
        string? brand,
        long baseUnitId,
        long purchaseUnitId,
        decimal purchaseConversionFactor,
        decimal minimumStock) =>
        new(name, code, brand, baseUnitId, purchaseUnitId, purchaseConversionFactor, minimumStock);

    public void Update(
        string name,
        string code,
        string? brand,
        long baseUnitId,
        long purchaseUnitId,
        decimal purchaseConversionFactor,
        decimal minimumStock,
        bool isActive)
    {
        Name = name.Trim();
        Code = code.Trim();
        Brand = string.IsNullOrWhiteSpace(brand) ? null : brand.Trim();
        BaseUnitId = baseUnitId;
        PurchaseUnitId = purchaseUnitId;
        PurchaseConversionFactor = purchaseConversionFactor;
        MinimumStock = minimumStock;
        Status = isActive;
    }

    public void Deactivate() => Status = false;

    public void Activate() => Status = true;
}
