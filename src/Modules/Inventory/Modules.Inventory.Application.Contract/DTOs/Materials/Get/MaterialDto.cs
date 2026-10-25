namespace Modules.Inventory.Application.Contract.DTOs.Materials;

public sealed class MaterialDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Brand { get; set; }
    public long BaseUnitId { get; set; }
    public string BaseUnitName { get; set; } = null!;
    public string BaseUnitSymbol { get; set; } = null!;
    public long PurchaseUnitId { get; set; }
    public string PurchaseUnitName { get; set; } = null!;
    public string PurchaseUnitSymbol { get; set; } = null!;
    public decimal PurchaseConversionFactor { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal CurrentStock { get; set; }
    public bool IsActive { get; set; }
}
