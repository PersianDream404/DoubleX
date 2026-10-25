using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Application.Contract.DTOs.Units;

public sealed class UnitDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public UnitType UnitType { get; set; }
    public bool IsActive { get; set; }
}
