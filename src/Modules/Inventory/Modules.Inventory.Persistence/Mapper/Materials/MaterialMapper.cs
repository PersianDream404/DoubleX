using System.Linq.Expressions;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using MaterialEntity = Modules.Inventory.Domain.Entities.Material;

namespace Modules.Inventory.Persistence.Mapper.Materials;

public static class MaterialMapper
{
    public static Expression<Func<MaterialEntity, MaterialDto>> ToGetAllDto() =>
        material => new MaterialDto
        {
            Id = material.Id,
            Name = material.Name,
            Code = material.Code,
            Brand = material.Brand,
            BaseUnitId = material.BaseUnitId,
            BaseUnitName = material.BaseUnit.Name,
            BaseUnitSymbol = material.BaseUnit.Symbol,
            PurchaseUnitId = material.PurchaseUnitId,
            PurchaseUnitName = material.PurchaseUnit.Name,
            PurchaseUnitSymbol = material.PurchaseUnit.Symbol,
            PurchaseConversionFactor = material.PurchaseConversionFactor,
            MinimumStock = material.MinimumStock,
            CurrentStock = material.StockBalance == null ? 0 : material.StockBalance.Quantity,
            IsActive = material.Status
        };

    public static Expression<Func<MaterialEntity, MaterialDto>> ToGetByIdDto() =>
        material => new MaterialDto
        {
            Id = material.Id,
            Name = material.Name,
            Code = material.Code,
            Brand = material.Brand,
            BaseUnitId = material.BaseUnitId,
            BaseUnitName = material.BaseUnit.Name,
            BaseUnitSymbol = material.BaseUnit.Symbol,
            PurchaseUnitId = material.PurchaseUnitId,
            PurchaseUnitName = material.PurchaseUnit.Name,
            PurchaseUnitSymbol = material.PurchaseUnit.Symbol,
            PurchaseConversionFactor = material.PurchaseConversionFactor,
            MinimumStock = material.MinimumStock,
            CurrentStock = material.StockBalance == null ? 0 : material.StockBalance.Quantity,
            IsActive = material.Status
        };
}
