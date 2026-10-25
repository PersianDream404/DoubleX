using Modules.Inventory.Domain.Entities;

namespace Modules.Inventory.Application.Contract.DTOs;

public sealed class CreateUnitRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public UnitType UnitType { get; set; }
}
