using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Application.Contract.DTOs.Units;

public sealed class UpdateUnitRequestDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public UnitType UnitType { get; set; }
    public bool IsActive { get; set; }
}
