namespace Modules.Inventory.Application.Contract.DTOs;

public sealed class CreateMaterialRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public long BaseUnitId { get; set; }
    public long PurchaseUnitId { get; set; }
    public decimal PurchaseConversionFactor { get; set; }
    public decimal MinimumStock { get; set; }
}
