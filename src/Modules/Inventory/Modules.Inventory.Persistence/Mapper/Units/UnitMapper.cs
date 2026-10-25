using System.Linq.Expressions;
using Modules.Inventory.Application.Contract.DTOs.Units;
using UnitEntity = Modules.Inventory.Domain.Entities.Unit;

namespace Modules.Inventory.Persistence.Mapper.Units;

public static class UnitMapper
{
    public static Expression<Func<UnitEntity, UnitDto>> ToGetAllDto() =>
        unit => new UnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Symbol = unit.Symbol,
            UnitType = unit.UnitType,
            IsActive = unit.Status
        };

    public static Expression<Func<UnitEntity, UnitDto>> ToGetByIdDto() =>
        unit => new UnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Symbol = unit.Symbol,
            UnitType = unit.UnitType,
            IsActive = unit.Status
        };
}
